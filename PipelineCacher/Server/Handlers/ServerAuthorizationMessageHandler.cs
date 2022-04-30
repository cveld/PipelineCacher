using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Identity.Web;
using PipelineCacher.Shared.Const;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace PipelineCacher.Server.Handlers
{
    // https://github.com/dotnet/aspnetcore/blob/a450cb69b5e4549f5515cdb057a68771f56cefd7/src/Components/WebAssembly/WebAssembly.Authentication/src/Services/AuthorizationMessageHandler.cs#L16
    public class ServerAuthorizationMessageHandler : DelegatingHandler
    {
        private readonly ITokenAcquisition tokenAcquisition;
        private readonly NavigationManager navigationManager;
        private string accessToken;
        private AuthenticationHeaderValue _cachedHeader;

        public ServerAuthorizationMessageHandler(ITokenAcquisition tokenAcquisition, NavigationManager navigationManager)
        {
            this.tokenAcquisition = tokenAcquisition;
            this.navigationManager = navigationManager;
        }

        protected override async Task<HttpResponseMessage>
          SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (accessToken == null)
            {
                accessToken = await tokenAcquisition.GetAccessTokenForUserAsync(new[] { Const.APIScope });
                _cachedHeader = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            request.Headers.Authorization = _cachedHeader;
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
