using Microsoft.AspNetCore.Components;
using System;
using System.Net.Http;

namespace PipelineCacher.Client.Services
{
    public class AnonymousClient
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly NavigationManager navigationManager;

        public AnonymousClient(IHttpClientFactory clientFactory, NavigationManager navigationManager)
        {
            this.clientFactory = clientFactory;
            this.navigationManager = navigationManager;
            httpClient = clientFactory.CreateClient(HttpClientNames.Anonymous);
            httpClient.BaseAddress = new Uri(navigationManager.BaseUri);
        }

        public HttpClient httpClient { get; set; }
    }
}
