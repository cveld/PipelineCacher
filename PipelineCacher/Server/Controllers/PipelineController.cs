using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PipelineCacher.Entities;
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

        [HttpGet("{id}/builds")]
        public async Task<object> GetPipelineBuilds(int id, [FromQuery][Required] int? PatId)
        {
            var pipeline = await context.Pipelines.FindAsync(id);
            var pat = await context.Pats.FindAsync(PatId);
            var collectionUri = $"https://dev.azure.com/{pipeline.OrganizationName}";
            VssConnection connection = new VssConnection(new Uri(collectionUri), new VssBasicCredential(String.Empty, pat.Token));
            var client = connection.GetClient<BuildHttpClient>();
            var builds = await client.GetBuildsAsync(pipeline.ProjectName, new int[] { pipeline.AzdoId });
            var timeline = await client.GetBuildTimelineAsync(pipeline.ProjectName, builds[0].Id);
            
            // Serialize output object to http response
            return ObjectToJson(new
            {
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

        [HttpPost("{id}/contexts/{contextid}/parseyaml")]
        public async Task<object> ParseYaml(int id, int contextid)
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
        public async Task<object> PopulateYaml(int id, int contextid, [FromQuery][Required] int? PatId)
        {
            var pipeline = await context.Pipelines.FindAsync(id);
            var pipelineContext = await context.PipelineContexts.FindAsync(contextid);
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

        async Task<VssConnection> GetConnection(int patId, string organizationName)
        {
            var pat = await context.Pats.FindAsync(patId);
            var collectionUri = $"https://dev.azure.com/{organizationName}";
            VssConnection connection = new VssConnection(new Uri(collectionUri), new VssBasicCredential(String.Empty, pat.Token));
            return connection;
        }

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
    }   
}
