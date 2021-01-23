using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PipelineCacher.Entities;
using PipelineCacher.Server.Utilities;
using PipelineCacher.Shared;
using PipelineCacher.Shared.Commands;
using PipelineCacher.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using PipelineCacher.Server.AzureDevOps;

namespace PipelineCacher.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PipelineController : ControllerBase
    {
        private readonly PipelineCacherDbContext context;

        public PipelineController(PipelineCacherDbContext context)
        {
            this.context = context;
        }
        [HttpGet]
        void GetPipelines()
        {

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Pipeline>> GetPipeline(int id)
        {
            return null;
        }

        // TODO; move shared populate code from pipelinecontext and pipelinebuild into a shared function
        [HttpPost("{id}/builds/{buildid}/populateyaml")]
        public async Task<object> PopulatePipelineBuildYaml(int id, int buildid, [FromQuery][Required] int? patId)
        {
            var pipeline = await context.Pipelines.FindAsync(id);
            if (pipeline == null) {
                return new NotFoundObjectResult($"Pipeline {id} cannot be found");
            }
            var pipelineBuildState = await context.PipelineStates.SingleOrDefaultAsync(s => s.AzdoBuildId == buildid);
            if (pipelineBuildState == null)
            {
                return new NotFoundObjectResult($"Build {buildid} cannot be found");
            }
            if (pipelineBuildState.Pipeline != pipeline)
            {
                return new BadRequestObjectResult($"Pipeline {id} does not match with pipeline build state {buildid}");
            }
            var connection = await GetConnection(patId.Value, pipeline.OrganizationName);
            var gitClient = connection.GetClient<GitHttpClient>();
            var item = await gitClient.GetItemAsync(
                pipeline.RepositoryId,
                pipelineBuildState.YamlPath,
                includeContent: true,
                versionDescriptor: new GitVersionDescriptor
                {
                    Version = pipelineBuildState.Commit,
                    VersionType = GitVersionType.Commit
                });
            var commitId = item.CommitId;

            //var sourceCode = new Sourcecode
            //{
            //    Content = item.Content
            //};
            //context.Sourcecode.Add(sourceCode);
            //await context.SaveChangesAsync();
            pipelineBuildState.SourcecodeTree = new SourcecodeTree();
            pipelineBuildState.SourcecodeTree.Pipeline = pipelineBuildState.SourcecodeTree.Pipeline.Add(pipelineBuildState.YamlPath, item.Content);
            await context.SaveChangesAsync();
            return new
            {
                commit = commitId
            };
        }
        /// <summary>
        /// Fetch main data of a pipeline build
        /// </summary>
        /// <param name="id"></param>
        /// <param name="buildid"></param>
        /// <param name="patId"></param>
        /// <returns></returns>
        [HttpPost("{id}/builds/{buildid}/populate")]
        public async Task<object> PopulatePipelineBuild(int id, int buildid, [FromQuery][Required] int? patId)
        {
            var pipeline = await context.Pipelines.FindAsync(id);
            var connection = await GetConnection(patId.Value, pipeline.OrganizationName);
            
            var client = connection.GetClient<CustomBuildHttpClient>();

            var pipelineState = await context.PipelineStates.SingleOrDefaultAsync(state => state.AzdoBuildId == buildid);
            if (pipelineState == null)
            {
                pipelineState = new PipelineState();
                context.PipelineStates.Add(pipelineState);
                pipelineState.AzdoBuildId = buildid;
            }
            var build = await client.GetCustomBuildAsync(pipeline.ProjectName, buildid);            
            if (build.Definition.Id != pipeline.AzdoId)
            {
                throw new ArgumentOutOfRangeException($"Build {buildid} is not related to pipeline {id} with Azure Pipelines id {pipeline.AzdoId}");
            }
            pipelineState.Pipeline = pipeline;
            var sourceBranch = build.SourceBranch;
            if (sourceBranch.StartsWith("refs/heads/"))
            {
                sourceBranch = sourceBranch.Substring("refs/heads/".Length);
            }
            pipelineState.Branch = sourceBranch;
            pipelineState.Commit = build.SourceVersion;
            pipelineState.Revision = build.Definition.Revision.Value;

            // Store template parameters
            var templateParameters = build.TemplateParameters;
            pipelineState.Parameters = ImmutableDictionary<string, string>.Empty;
            foreach (var parameter in templateParameters)
            {
                pipelineState.Parameters = pipelineState.Parameters.Add(parameter.Key, parameter.Value);
            }

            var pipelineDefinitionRev = await client.GetDefinitionAsync(pipeline.ProjectName, build.Definition.Id, build.Definition.Revision);

            var process = pipelineDefinitionRev.Process as YamlProcess;
            pipelineState.YamlPath = process.YamlFilename;

            // Process parameters
            var outputParameters = ImmutableDictionary<string, string>.Empty;
            //var inputParameters = build.temp

            // Process timeline data into Stage list
            var timeline = await client.GetBuildTimelineAsync(pipeline.ProjectName, buildid);
            var stages = timeline.Records.Where(r => r.RecordType == "Stage").OrderBy(r => r.Order);
            pipelineState.Stages = ImmutableList<Stage>.Empty;

            foreach (var stage in stages)
            {
                pipelineState.Stages = pipelineState.Stages.Add(new Stage
                {
                    PipelineRunId = pipelineState.AzdoBuildId,
                    Status = AzureDevOpsMapper.MapStatus(stage.Result.Value)
                });
            }

            await context.SaveChangesAsync();

            return ObjectToJson(new
            {
                pipelineState,
                PipelineYamlPath = pipeline.YamlPath,                
                Stages = stages,
                Timeline = timeline,
                Build = build
            });
        }

        [HttpGet("{id}/builds")]
        public async Task<object> GetPipelineBuilds(int id, [FromQuery][Required] int? patId)
        {
            var pipeline = await context.Pipelines.FindAsync(id);                       
            var connection = await GetConnection(patId.Value, pipeline.OrganizationName);
            var client = connection.GetClient<BuildHttpClient>();
            var builds = await client.GetBuildsAsync(pipeline.ProjectName, new int[] { pipeline.AzdoId });
            object timeline = null; // await client.GetBuildTimelineAsync(pipeline.ProjectName, builds[0].Id);
            
            // Serialize output object to http response
            return ObjectToJson(new
            {
                Pipeline = pipeline,
                Timeline = timeline,
                Builds = builds                
            });           
        }

        [HttpGet("{id}/compare/{repoRef}")]
        public async Task Compare(int id, string repoRef)
        {

        }

        [HttpPost]
        public async Task<ActionResult<Pipeline>> CreatePipeline(CreatePipelineCommand command)
        {
            var pipeline = new Pipeline
            {
                ProjectName = command.ProjectName,
                OrganizationName = command.OrganizationName,
                Name = command.ProjectName,
                AzdoId = command.AzdoPipelineId.Value
            };
            context.Pipelines.Add(pipeline);
            await context.SaveChangesAsync();
            return CreatedAtAction("GetPipeline", new { id = pipeline.Id }, pipeline);
        }

        void Test()
        {
            // User applies a build from the history towards a PipelineContext
            // This creates a new PipelineState or reuses it
        }

        void CheckDesiredState()
        {

        }

        /// <summary>
        /// Explicit serializer from object to json string.
        /// Required because the build-in serializer does not access properties in inherited classes
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        string ObjectToJson(object o)
        {
            var json = JsonConvert.SerializeObject(
                o,
                Newtonsoft.Json.Formatting.Indented,
                new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return json;
        }

        /// <summary>
        /// Parses the downloaded yaml for the specific pipeline build and extracts the content per separate stage
        /// </summary>
        /// <param name="id"></param>
        /// <param name="contextid"></param>
        /// <returns></returns>
        [HttpPost("{id}/builds/{buildid}/parseyaml")]
        public async Task<object> ParsePipelineBuildYaml(int id, int buildid)
        {
            var pipeline = await context.Pipelines.FindAsync(id);
            if (pipeline == null)
            {
                return new BadRequestObjectResult($"pipeline {id} not found");
            }
            var pipelineState = await context.PipelineStates.SingleOrDefaultAsync(s => s.AzdoBuildId == buildid);
            if (pipelineState == null)
            {
                return new BadRequestObjectResult($"pipeline build {buildid} not found");
            }

            var s = pipelineState.SourcecodeTree.Pipeline[pipelineState.YamlPath];
            var sr = new StringReader(s);
            var stream = new YamlStream();
            stream.Load(sr);
            var rootNode = stream.Documents[0].RootNode;
            var inputStages = rootNode.SequenceValue("stages");
            var stages = pipelineState.Stages;
            if (inputStages != null)
            {
                if (inputStages.Children.Count != stages.Count)
                {
                    return new BadRequestObjectResult($"Number of stages in yaml input file is {inputStages.Children.Count} which is not equal to fetched number of stages from pipeline build timeline being {stages.Count}");
                }
                for (int i = 0; i < inputStages.Children.Count; i++) {
                    var inputStage = inputStages[i];
                    var stageName = inputStage.ScalarValue("stage")?.Value;
                    var stageDisplayname = inputStages.ScalarValue("displayName")?.Value;
                    stages = stages.SetItem(i, stages[i] with 
                    {
                        Content = PipelineCacher.Shared.Utilities.Yaml.YamlNodeToString(inputStage)                        
                    });
                }
            }

            pipelineState.Stages = stages;
            await context.SaveChangesAsync();
            return ObjectToJson(stages);
        }


        /// <summary>
        /// Parse Pipeline Context's fetched yaml
        /// To be explored whether we should fetch additional yaml based on findings here, or whether everything is separately defined
        /// </summary>
        /// <param name="id"></param>
        /// <param name="contextid"></param>
        /// <returns></returns>
        [HttpPost("{id}/contexts/{contextid}/parseyaml")]
        public async Task<object> ParsePipelineContextYaml(int id, int contextid)
        {
            var pipeline = await context.Pipelines.FindAsync(id);
            var pipelineContext = await context.PipelineContexts.FindAsync(contextid);
            
            var s = pipelineContext.SourcecodeTree.Pipeline[pipeline.YamlPath];
            var deserializer = new DeserializerBuilder()
                //.JsonCompatible()
                //.WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            /*
            var stringReader = new StringReader(s);
            var yamlpipeline = deserializer.Deserialize(stringReader);
            var jobject = JObject.FromObject(yamlpipeline);
            var inputStages = jobject["stages"]?.Value<JArray>();
            var stages = ImmutableList<Stage>.Empty;
            if (inputStages != null)
            {
                foreach (var inputStage in inputStages)
                {
                    var stageName = inputStages["stage"];
                    var stageDisplayname = inputStages["displayName"];
                    stages = stages.Add(new Stage
                    {                        
                        Content = inputStage.ToString(),
                        Status = StatusEnum.NotRun
                    });
                }
            }
            */
            var sr = new StringReader(s);
            var stream = new YamlStream();
            stream.Load(sr);
            var rootNode = stream.Documents[0].RootNode;
            var inputStages = rootNode.SequenceValue("stages");
            var stages = ImmutableList<Stage>.Empty;
            if (inputStages != null)
            {
                foreach (var inputStage in inputStages)
                {
                    var stageName = inputStage.ScalarValue("stage")?.Value;
                    var stageDisplayname = inputStages.ScalarValue("displayName")?.Value;
                    stages = stages.Add(new Stage
                    {
                        Content = PipelineCacher.Shared.Utilities.Yaml.YamlNodeToString(inputStage),
                        Status = StatusEnum.NotRun
                    });
                }
            }

            pipelineContext.Stages = stages;
            await context.SaveChangesAsync();
            return ObjectToJson(stages);
        }

        [HttpPost("{id}/contexts/{contextid}/populateyaml")]
        public async Task<object> PopulatePipelineContextYaml(int id, int contextid, [FromQuery][Required] int? PatId)
        {
            var pipeline = await context.Pipelines.FindAsync(id);
            var pipelineContext = await context.PipelineContexts.FindAsync(contextid);
            if (pipelineContext.Pipeline != pipeline)
            {
                throw new ArgumentException($"Pipeline {id} does not match with pipeline context {contextid}");
            }
            var connection = await GetConnection(PatId.Value, pipeline.OrganizationName);
            var gitClient = connection.GetClient<GitHttpClient>();
            var item = await gitClient.GetItemAsync(
                pipeline.RepositoryId, 
                pipeline.YamlPath, 
                includeContent: true,
                versionDescriptor: new GitVersionDescriptor
            {
                Version = pipelineContext.TargetBranch,
                VersionType = GitVersionType.Branch
            });
            var commitId = item.CommitId;

            //var sourceCode = new Sourcecode
            //{
            //    Content = item.Content
            //};
            //context.Sourcecode.Add(sourceCode);
            //await context.SaveChangesAsync();
            pipelineContext.SourcecodeTree = new SourcecodeTree();
            pipelineContext.SourcecodeTree.Pipeline = pipelineContext.SourcecodeTree.Pipeline.Add(pipeline.YamlPath, item.Content);
            pipelineContext.LastChecked = DateTime.Now;
            pipelineContext.CommitId = commitId;
            await context.SaveChangesAsync();
            
            // build graph of sourcecode files
            // 

            /*
             { "pipeline": {
                     "/yamlpath.yaml": "referenceid"                
                }
            }
             */
            return ObjectToJson(new
            {
                PipelineContext = pipelineContext,
                YamlFile = item
            });
        }

        [HttpPost("{id}/contexts")]
        public async Task<ActionResult<PipelineContext>> CreateContext(CreatePipelineContextCommand command)
        {
            var pipeline = await context.Pipelines.FindAsync(command.PipelineId);
            var pipelineContext = new PipelineContext
            {
                Environment = command.Environment,
                Pipeline = pipeline,
                TargetBranch = command.TargetBranch
            };
            context.PipelineContexts.Add(pipelineContext);
            await context.SaveChangesAsync();
            return CreatedAtAction("GetPipeline", new { id = pipeline.Id }, pipelineContext);

        }

        async Task<(Pipeline, PipelineContext)> QueryPipelineAndPipelineContextAsync(int id, int contextid)
        {
            var pipeline = await context.Pipelines.FindAsync(id);
            if (pipeline == null)
            {
                throw new ArgumentOutOfRangeException($"Could not find a pipeline for id {id}");
            }
            var pipelineContext = await context.PipelineContexts.FindAsync(contextid);
            if (pipelineContext == null)
            {
                throw new ArgumentOutOfRangeException($"Could not find a pipeline context for id {contextid}");
            }
            if (pipelineContext.Pipeline != pipeline)
            {
                throw new ArgumentException($"Pipeline {id} does not match with pipeline context {contextid}");
            }
            return (pipeline, pipelineContext);
        }

        [HttpPost("{id}/contexts/{contextid}/parameters")]
        public async Task<object> SetPipelineContextParameters(int id, int contextid, SetPipelineContextParametersCommand command)
        {
            (var pipeline, var pipelineContext) = await QueryPipelineAndPipelineContextAsync(id, contextid);

            pipelineContext.Parameters = ImmutableDictionary<string, string>.Empty;
            if (command.Parameters == null)
            {
                return new BadRequestObjectResult("Parameters not provided");
            }
            foreach(var parameter in command.Parameters)
            {
                pipelineContext.Parameters = pipelineContext.Parameters.Add(parameter.Key, parameter.Value);
            }
            await context.SaveChangesAsync();

            return ObjectToJson(new
            {
                PipelineContext = pipelineContext
            });
        }

        async Task<VssConnection> GetConnection(int patId, string organizationName)
        {
            var pat = await context.Pats.FindAsync(patId);
            var collectionUri = $"https://dev.azure.com/{organizationName}";
            VssConnection connection = new VssConnection(new Uri(collectionUri), new VssBasicCredential(String.Empty, pat.Token));
            return connection;
        }

        /// <summary>
        /// Fetch pipeline definition main data from Azure Pipelines
        /// </summary>
        /// <param name="id"></param>
        /// <param name="PatId"></param>
        /// <returns></returns>
        [HttpPost("{id}/populate")]
        public async Task<object> PopulatePipeline(int id, [FromQuery] [Required] int? PatId)
        {
            var pipeline = await context.Pipelines.FindAsync(id);            
            var connection = await GetConnection(PatId.Value, pipeline.OrganizationName);
            var client = connection.GetClient<BuildHttpClient>();
            var definition = await client.GetDefinitionAsync(pipeline.ProjectName, pipeline.AzdoId); // , revision: 2
            var builds = await client.GetBuildsAsync(pipeline.ProjectName, new int[] { pipeline.AzdoId });
                   
            var sourceVersion = builds[0].SourceVersion;
            var sourceBranch = builds[0].SourceBranch;
            if (definition.Process.Type != ProcessType.Yaml)
            {
                throw new ArgumentOutOfRangeException($"ProcessType {definition.Process.Type} not supported. Only type 2 (yaml) is supported");
            }
            YamlProcess process = definition.Process as YamlProcess;
            pipeline.YamlPath = process.YamlFilename;
            pipeline.RepositoryId = definition.Repository.Id;
            pipeline.Revision = definition.Revision.Value;

            await context.SaveChangesAsync();

            if (sourceBranch.StartsWith("refs/heads/"))
            {
                sourceBranch = sourceBranch.Substring("refs/heads/".Length);
            }
            var repositoryId = builds[0].Repository.Id;
            var gitClient = connection.GetClient<GitHttpClient>();
            
            GitBaseVersionDescriptor gitBaseVersionDescriptor = new GitBaseVersionDescriptor
            {
                Version = sourceVersion,
                VersionType = GitVersionType.Commit
            };
            GitTargetVersionDescriptor gitTargetVersionDescriptor = new GitTargetVersionDescriptor
            {
                Version = sourceBranch,
                VersionType = GitVersionType.Branch
            };
            var commitDiffs = await gitClient.GetCommitDiffsAsync(repositoryId, baseVersionDescriptor: gitBaseVersionDescriptor, targetVersionDescriptor: gitTargetVersionDescriptor);

            // Serialize output object to http response
            return ObjectToJson(new
            {
                Pipeline = pipeline,
                CommitDiffs = commitDiffs,
                Builds = builds,
                Definition = definition
            });
            
        }

        /// <summary>
        /// Brings back the PipelineContext to a clean state
        /// Only the pipeline definition including stage dependencies are kept
        /// Any results of processed pipeline runs (PipelineState records) are removed
        /// </summary>
        [HttpPost("{id}/contexts/{contextid}/clearstagesstatus")]
        public async Task<object> ClearPipelineContextStagesStatusAsync(int id, int contextid)
        {
            (var pipeline, var pipelineContext) = await QueryPipelineAndPipelineContextAsync(id, contextid);
            for (int i = 0; i < pipelineContext.Stages.Count; i++)
            {
                var stage = pipelineContext.Stages[i];
                pipelineContext.Stages = pipelineContext.Stages.SetItem(i, stage with {
                    Status = StatusEnum.NotRun,
                    PipelineRunId = null,
                    ValidationTimestamp = null
                });
            }
            await context.SaveChangesAsync();
            return ObjectToJson(new
            {
                Pipeline = pipeline,
                PipelineContext = pipelineContext
            });
        }
    }   
}
