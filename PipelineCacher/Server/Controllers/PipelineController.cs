using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using PipelineCacher.Entities;
using PipelineCacher.Shared;
using PipelineCacher.Shared.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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

            // Serialize output object to http response
            var result = new
            {
                Builds = builds                
            };
            var json = JsonConvert.SerializeObject(
            result,
            Newtonsoft.Json.Formatting.Indented,
            new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return json;
        }

        [HttpGet("{id}/compare/{repoRef}")]
        public async Task Compare(int id, string repoRef)
        {

        }

        [HttpPost]
        public async Task<ActionResult<Pipeline>> PostPipeline(PipelinePostModel model)
        {
            var pipeline = new Pipeline
            {
                ProjectName = model.ProjectName,
                OrganizationName = model.OrganizationName,
                Name = model.ProjectName,
                AzdoId = model.AzdoPipelineId.Value
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

        [HttpPost("{id}/contexts")]
        public async Task CreateContext(CreatePipelineContextCommand command)
        {
            var pipeline = await context.Pipelines.FindAsync(command.PipelineId);
            var pipelineContext = new PipelineContext
            {
                Environment = command.Environment,
                Pipeline = pipeline,
                TargetBranch = command.TargetBranch
            };
            context.PipelineContexts.Add(pipelineContext);
        }

        [HttpGet("populate/{id}")]
        public async Task<object> PopulatePipeline(int id, [FromQuery] [Required] int? PatId)
        {
            var pipeline = await context.Pipelines.FindAsync(id);
            var pat = await context.Pats.FindAsync(PatId);
            var collectionUri = $"https://dev.azure.com/{pipeline.OrganizationName}";
            VssConnection connection = new VssConnection(new Uri(collectionUri), new VssBasicCredential(String.Empty, pat.Token));
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
            var result = new
            {
                commitDiffs,
                Builds = builds,
                Definition = definition
            };            
            var json = JsonConvert.SerializeObject(
            result,
            Newtonsoft.Json.Formatting.Indented,
            new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return json;
        }
    }   
}
