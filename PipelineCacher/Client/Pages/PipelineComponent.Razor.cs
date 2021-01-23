using Microsoft.AspNetCore.Components;
using PipelineCacher.Client.Models;
using PipelineCacher.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace PipelineCacher.Client.Pages
{
    public partial class PipelineComponent
    {
        [Parameter]
        public string Id { get; set; }
        [Inject] HttpClient Http { get; set; }

        RestCallStatusEnum pipeline_loadingStatus = RestCallStatusEnum.NotStarted;
        string ResultMessage;
        Pipeline pipeline;

        protected override async Task OnInitializedAsync()
        {
            pipeline_loadingStatus = RestCallStatusEnum.Getting;
            StateHasChanged();

            try
            {
                var result = await Http.GetFromJsonAsync<Pipeline>($"api/pipeline/{Id}");
                pipeline = result;
                pipeline_loadingStatus = RestCallStatusEnum.Ok;
            }
            catch (Exception e)
            {
                pipeline_loadingStatus = RestCallStatusEnum.Failed;
                ResultMessage = e.ToString();//.Message;
            }
        }
    }
}
