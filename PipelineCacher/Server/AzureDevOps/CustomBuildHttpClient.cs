using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace PipelineCacher.Server.AzureDevOps
{
    public class CustomBuildHttpClient : BuildHttpClient
    {
        private readonly Uri baseUrl;
        private readonly VssCredentials credentials;

        public CustomBuildHttpClient(Uri baseUrl, VssCredentials credentials) : base(baseUrl, credentials)
        {
            this.baseUrl = baseUrl;
            this.credentials = credentials;
        }

        public CustomBuildHttpClient(Uri baseUrl, VssCredentials credentials, VssHttpRequestSettings settings) : base(baseUrl, credentials, settings)
        {
        }

        public CustomBuildHttpClient(Uri baseUrl, VssCredentials credentials, params DelegatingHandler[] handlers) : base(baseUrl, credentials, handlers)
        {
        }

        public CustomBuildHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler) : base(baseUrl, pipeline, disposeHandler)
        {
        }

        public CustomBuildHttpClient(Uri baseUrl, VssCredentials credentials, VssHttpRequestSettings settings, params DelegatingHandler[] handlers) : base(baseUrl, credentials, settings, handlers)
        {
        }

        /// <summary>
        /// Gets a build
        /// </summary>
        /// <param name="project">Project ID or project name</param>
        /// <param name="buildId"></param>
        /// <param name="propertyFilters"></param>
        /// <param name="userState"></param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        public virtual Task<CustomBuild> GetCustomBuildAsync(string project, int buildId, string propertyFilters = null, object userState = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Expected O, but got Unknown
			HttpMethod method = new HttpMethod("GET");
			Guid locationId = new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf");
			object routeValues = new
			{
				project,
				buildId
			};
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			if (propertyFilters != null)
			{
                Microsoft.VisualStudio.Services.Common.UriExtensions.Add((IList<KeyValuePair<string, string>>)list, "propertyFilters", propertyFilters);
			}
			return SendAsync<CustomBuild>(method, locationId, routeValues, new ApiResourceVersion(6.0, 0), null, list, userState, cancellationToken);
		}

	}
}
