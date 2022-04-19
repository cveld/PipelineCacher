using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using PipelineCacher.Client.Models;
using PipelineCacher.Entities;
using PipelineCacher.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PipelineCacher.Client.Pages
{
    public partial class AzdoAuthentication
    {
        [Parameter]
        public string Action { get; set; }

        protected async override Task OnInitializedAsync()
        {
            base.OnInitialized();
            Enum.TryParse(typeof(AzdoAuthenticationEnum), Action, true, out var azdoAuthenticationEnum);
            switch (azdoAuthenticationEnum)
            {
                case AzdoAuthenticationEnum.Authorize:
                    await Authorize();
                    break;
                case AzdoAuthenticationEnum.Callback:
                    var query = new Uri(navigationManager.Uri).Query;

                    if (QueryHelpers.ParseQuery(query).TryGetValue("code", out var code) 
                        && QueryHelpers.ParseQuery(query).TryGetValue("state", out var state) && Guid.TryParse(state, out var guid))
                    {
                        await Callback(code, guid);
                    }
                    else
                    {
                        Result = "input parameters incorrect";
                    }
                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Action));
            }
        }

        private async Task Authorize()
        {
            Guid state = Guid.NewGuid();

            await sessionStorage.SetItemAsync($"azdoauthentication-{state}", new AzdoToken { IsPending = true });
            navigationManager.NavigateTo(GetAuthorizationUrl(state.ToString()));
        }

        private String GetAuthorizationUrl(String state)
        {
            UriBuilder uriBuilder = new UriBuilder(configuration["Azdo:AuthUrl"]);
            var queryParams = HttpUtility.ParseQueryString(uriBuilder.Query ?? String.Empty);

            queryParams["client_id"] = configuration["Azdo:ClientAppId"];
            queryParams["response_type"] = "Assertion";
            queryParams["state"] = state;
            queryParams["scope"] = configuration["Azdo:Scope"];
            queryParams["redirect_uri"] = configuration["Azdo:CallbackUrl"];

            uriBuilder.Query = queryParams.ToString();

            return uriBuilder.ToString();
        }

        public string Result { get; set; }
        /// <summary>
        /// Callback action. Invoked after the user has authorized the app.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        protected async Task Callback(string code, Guid state)
        {
            String error = await ValidateCallbackValues(code, state.ToString());
            if (!string.IsNullOrEmpty(error))
            {
                Result = $"Error: {error}";
            }
            else
            {
                Result = "Validation sucessful. We can now pass the token to the backend";
                var result = await httpClient.PostAsJsonAsync("api/azdoauth", code);
                var azdoTokenProjection = await result.Content.ReadFromJsonAsync<AzdoTokenProjection>();
                navigationManager.NavigateTo("profile");
            }
        }

        private async Task<string> ValidateCallbackValues(String code, String state)
        {
            string error = null;

            if (String.IsNullOrEmpty(code))
            {
                error = "Invalid auth code";
            }
            else
            {
                Guid authorizationRequestKey;
                if (!Guid.TryParse(state, out authorizationRequestKey))
                {
                    error = "Invalid authorization request key";
                }
                else
                {                    
                    var tokenModel = await sessionStorage.GetItemAsync<AzdoToken>($"azdoauthentication-{state}");

                    
                    if (tokenModel == null)
                    {
                        error = "Unknown authorization request key";
                    }
                    else if (!tokenModel.IsPending)
                    {
                        error = "Authorization request key already used";
                    }
                    else
                    {
                        tokenModel.IsPending = false; // mark the state value as used so it can't be reused
                        await sessionStorage.SetItemAsync<AzdoToken>($"azdoauthentication-{state}", tokenModel);
                    }
                }
            }

            return error;
        }

    }
}
