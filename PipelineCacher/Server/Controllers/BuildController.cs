using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using PipelineCacher.Entities;

namespace PipelineCacher.Server.Controllers
{
    // a PipelineCacher PipelineState maps to some extent one-to-one to a AzDO pipeline run
    [ApiController]
    [Route("api/[controller]")]
    public class BuildController : ControllerBase
    {
        private readonly PipelineCacherConfig pipelineCacherConfig;
        private readonly PipelineCacherDbContext context;

        public BuildController(IOptions<PipelineCacherConfig> pipelineCacherConfig, PipelineCacherDbContext context)
        {
            this.pipelineCacherConfig = pipelineCacherConfig.Value;
            this.context = context;
        }
        [HttpGet]
        public async Task<string> Get()
        {
            var collectionUri = "https://dev.azure.com/carlintveld";
            VssConnection connection = new VssConnection(new Uri(collectionUri), new VssBasicCredential(String.Empty, pipelineCacherConfig.AzureDevOpsPAT));
            var client = connection.GetClient<BuildHttpClient>();
            // https://dev.azure.com/carlintveld/lucas-demo/_build?definitionId=28
            var definition = await client.GetDefinitionAsync("lucas-demo", 28);

            // https://dev.azure.com/{organization}/{project}/_apis/build/builds/{buildId}?api-version=6.0            
            var builds = await client.GetBuildsAsync("lucas-demo", new int[] { 28 });
            //var build = await client.GetBuildAsync("lucas-demo", builds[0].);

            return builds[0].SourceVersion;            
        }

        [HttpPost("{id}/cachepipelineruns")]
        public async Task<object> CachePipelineRuns(int id, [FromQuery][Required] int? PatId)
        {
            var pipeline = await context.Pipelines.FindAsync(id);
            if (pipeline == null)
            {
                return new NotFoundObjectResult($"Pipeline {id} cannot be found");
            }

            throw new NotImplementedException(nameof(CachePipelineRuns));
        }
    }
}
