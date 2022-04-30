using Microsoft.AspNetCore.Components;
using PipelineCacher.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PipelineCacher.Client.Services
{
    public class ApiServerBlazorWasmHttpClient: IApiServerHttpClient
    {
        public ApiServerBlazorWasmHttpClient(IHttpClientFactory httpClientFactory, NavigationManager navigationManager)
        {
            AnonymousHttpClient = httpClientFactory.CreateClient(HttpClientNames.Anonymous);
            AuthorizedHttpClient = httpClientFactory.CreateClient(HttpClientNames.Authorized);            
            //HttpClient.BaseAddress = new Uri(NavigationManager.BaseUri);
        }

        public HttpClient AuthorizedHttpClient { get; }
        public HttpClient AnonymousHttpClient { get; }
        public NavigationManager NavigationManager { get; }
    }
}
