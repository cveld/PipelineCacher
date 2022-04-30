using Microsoft.AspNetCore.Components;
using PipelineCacher.Client.Models;
using PipelineCacher.Client.Services;
using PipelineCacher.Entities;
using PipelineCacher.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace PipelineCacher.Client.Pages
{
    public partial class PipelineContextComponent
    {
        [Parameter]
        public string Id { get; set; }
        [Parameter]
        public string ContextId { get; set; }

        [Inject] IApiServerHttpClient ApiServerHttpClient { get; set; }

        RestCallStatusEnum pipelineContext_loadingStatus = RestCallStatusEnum.NotStarted;
        PipelineContext pipelineContext;
        List<AzdoPipelineRun> azdoPipelineRuns;

        string ResultMessage;

        protected override async Task OnInitializedAsync()
        {
            pipelineContext_loadingStatus = RestCallStatusEnum.Getting;
            StateHasChanged();
            
            try
            {
                var result = await ApiServerHttpClient.AnonymousHttpClient.GetFromJsonAsync<PipelineContext>($"api/pipeline/{Id}/contexts/{ContextId}");
                pipelineContext = result;
                pipelineContext_loadingStatus = RestCallStatusEnum.Ok;
            }
            catch (Exception e)
            {
                pipelineContext_loadingStatus = RestCallStatusEnum.Failed;
                ResultMessage = e.ToString();//.Message;
            }
        }

        RestCallStatusEnum parseYaml_loadingStatus = RestCallStatusEnum.NotStarted;
        async Task ParseYamlClickedAsync()
        {
            parseYaml_loadingStatus = RestCallStatusEnum.Posting;
            StateHasChanged();
            var result = await ApiServerHttpClient.AnonymousHttpClient.PostAsync($"api/pipeline/{Id}/contexts/{ContextId}/parseyaml", null);
            parseYaml_loadingStatus = RestCallStatusEnum.Ok;
            var updatedPipelineContext = await result.Content.ReadFromJsonAsync<PipelineContext>();
            pipelineContext = updatedPipelineContext;
        }

        async Task GetPipelineRuns()
        {
            var result = await ApiServerHttpClient.AnonymousHttpClient.GetAsync($"api/pipeline/{Id}/builds?patid=1");
            azdoPipelineRuns = await result.Content.ReadFromJsonAsync<List<AzdoPipelineRun>>();
        }
    }
}
