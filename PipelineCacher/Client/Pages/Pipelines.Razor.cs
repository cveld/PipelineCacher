using BlazorContextMenu;
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
    public partial class Pipelines
    {
        [Parameter]
        public string Id { get; set; }
        [Inject] IApiServerHttpClient ApiServerHttpClient { get; set; }
        
        RestCallStatusEnum pipelines_loadingStatus = RestCallStatusEnum.NotStarted;
        List<PipelinesItemModel> pipelines;
        string ResultMessage;        
        PipelinesTableModel pipelinesTableModel = new PipelinesTableModel();

        protected override async Task OnInitializedAsync()
        {
            pipelines_loadingStatus = RestCallStatusEnum.Getting;
            StateHasChanged();

            try
            {
                var result = await ApiServerHttpClient.AnonymousHttpClient.GetFromJsonAsync<Pipeline[]>($"api/pipeline");
                pipelines = result.Select(p => new PipelinesItemModel
                {
                    Pipeline = p                    
                }).ToList();
                pipelines_loadingStatus = RestCallStatusEnum.Ok;
            }
            catch (Exception e)
            {
                pipelines_loadingStatus = RestCallStatusEnum.Failed;
                ResultMessage = e.ToString();//.Message;
            }
        }

        void OpenInAzurePipelinesClicked()
        {
            if (pipelinesTableModel.SelectedPipeline == null) return;
            var pipeline = pipelines.Find(p => p.Pipeline.Id == pipelinesTableModel.SelectedPipeline);
            NavigateToAzureDevOpsPipeline(pipeline.Pipeline);
        }

        void NavigateToAzureDevOpsPipeline(Pipeline pipeline)
        {            
            NavigationManager.NavigateTo($"https://dev.azure.com/{pipeline.OrganizationName}/{pipeline.ProjectName}/_build?definitionId={pipeline.AzdoId}");
        }
        
        async Task PullDataFromAzurePipelinesClicked(ItemClickEventArgs e)
        {
            var pipelineItem = e.Data as PipelinesItemModel;
            var pipeline = pipelineItem.Pipeline;
            pipelineItem.LoadingStatus = RestCallStatusEnum.Posting;
            StateHasChanged();
            var result = await ApiServerHttpClient.AnonymousHttpClient.PostAsync($"api/pipeline/{pipeline.Id}/populate?PatId=1", null);
            pipelineItem.LoadingStatus = RestCallStatusEnum.Ok;
            var updatedPipeline = await result.Content.ReadFromJsonAsync<Pipeline>();
            pipelineItem.Pipeline = updatedPipeline;
        }
    }
}
