using Microsoft.AspNetCore.Components;
using PipelineCacher.Client.Models;
using PipelineCacher.Client.Services;
using PipelineCacher.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace PipelineCacher.Client.Pages
{
    public partial class PipelineContexts
    {
        [Parameter]
        public string Id { get; set; }
        [Parameter]
        public string ContextId { get; set; }

        [Inject] IApiServerHttpClient ApiServerHttpClient { get; set; }

        RestCallStatusEnum pipelineContexts_loadingStatus = RestCallStatusEnum.NotStarted;
        List<PipelineContext> pipelineContexts;

        string ResultMessage;

        protected override async Task OnInitializedAsync()
        {
            pipelineContexts_loadingStatus = RestCallStatusEnum.Getting;
            StateHasChanged();
            
            try
            {
                var result = await ApiServerHttpClient.AnonymousHttpClient.GetFromJsonAsync<PipelineContext[]>($"api/pipeline/{Id}/contexts");
                pipelineContexts = new List<PipelineContext>(result);
                pipelineContexts_loadingStatus = RestCallStatusEnum.Ok;
            }
            catch (Exception e)
            {
                pipelineContexts_loadingStatus = RestCallStatusEnum.Failed;
                ResultMessage = e.ToString();//.Message;
            }
        }
    }
}
