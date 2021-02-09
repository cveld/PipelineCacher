using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using PipelineCacher.Client.Models;
using PipelineCacher.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PipelineCacher.Client.Pages
{
    public partial class Organizations
    {
        [Inject] HttpClient Http { get; set; }
        [Inject] IModalService Modal { get; set; }


        List<Organization> organizations;
        Dictionary<Organization, RestCallStatusEnum> organization_loadingStatus = new Dictionary<Organization, RestCallStatusEnum>();
        RestCallStatusEnum addOrganization_loadingStatus = RestCallStatusEnum.NotStarted;
        RestCallStatusEnum organizations_loadingStatus = RestCallStatusEnum.NotStarted;

        protected override async Task OnInitializedAsync()
        {
            organizations_loadingStatus = RestCallStatusEnum.Getting;
            StateHasChanged();
            try
            {
                var result = await Http.GetFromJsonAsync<Organization[]>("api/organizations");
                organizations = new List<Organization>(result);
                organizations_loadingStatus = RestCallStatusEnum.Ok;
            }
            catch (Exception e)
            {
                organizations_loadingStatus = RestCallStatusEnum.Failed;
                ResultMessage = e.Message;
            }
        }

        string ResultMessage;

        async Task AddOrganization()
        {
            var result = await Modal.Show<Features.Organizations.Add>("Add an organization").Result;
            if (!result.Cancelled)
            {
                var org = (Organization)result.Data;
                addOrganization_loadingStatus = RestCallStatusEnum.Posting;
                StateHasChanged();
                var task = Task.Delay(1000);
                var response = await Http.PostAsJsonAsync("api/organizations", org);

                ResultMessage = response.StatusCode.ToString();

                if (response.IsSuccessStatusCode)
                {                    
                    var orgresult = await response.Content.ReadFromJsonAsync<Organization>();
                    await task;
                    organizations.Add(orgresult);
                    addOrganization_loadingStatus = RestCallStatusEnum.Ok;
                }
                else
                {
                    addOrganization_loadingStatus = RestCallStatusEnum.Failed;                    
                }
            }
        }

        async Task DeleteOrganization(Organization organization)
        {
            var result = await Modal.Show<Features.General.Confirmation>("Are you sure").Result;
            if (!result.Cancelled)
            {
                organization_loadingStatus[organization] = RestCallStatusEnum.Deleting;
                StateHasChanged();
                var taskdelay = Task.Delay(1000);
                
                var response = await Http.DeleteAsync($"api/organizations/{organization.Id}");
                await taskdelay;
                if (response.IsSuccessStatusCode)
                {
                    organizations.Remove(organization);
                }
                else
                {
                    organization_loadingStatus[organization] = RestCallStatusEnum.Failed;
                }
            }
        }
    }
}
