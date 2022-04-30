using Microsoft.AspNetCore.Components;
using PipelineCacher.Client.Services;
using PipelineCacher.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Implementation for IApiServerHttpClient for Blazor Server
/// </summary>
namespace PipelineCacher.Client.Services
{
    public class ApiServerBlazorServerHttpClient: IApiServerHttpClient
    {
        public ApiServerBlazorServerHttpClient(IHttpClientFactory httpClientFactory, NavigationManager navigationManager)
        {
            AnonymousHttpClient = httpClientFactory.CreateClient(HttpClientNames.Anonymous);
            AuthorizedHttpClient = httpClientFactory.CreateClient(HttpClientNames.Authorized);
            NavigationManager = navigationManager;
            AnonymousHttpClient.BaseAddress = new Uri(NavigationManager.BaseUri);
            AuthorizedHttpClient.BaseAddress = new Uri(NavigationManager.BaseUri);
        }

        public HttpClient AuthorizedHttpClient { get; }
        public HttpClient AnonymousHttpClient { get; }
        public NavigationManager NavigationManager { get; }
    }
}
