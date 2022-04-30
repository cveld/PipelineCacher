using PipelineCacher.Client.Services;
using PipelineCacher.Shared;
using PipelineCacher.Shared.Const;
using System.Net.Http;
using System.Net.Http.Json;

namespace PipelineCacher.ClientServices
{
    public class UserProfileAgent
    {
        IApiServerHttpClient apiServerHttpClient;

        public UserProfileAgent(IApiServerHttpClient apiServerHttpClient, IMicrosoftIdentityConsentAndConditionalAccessHandler microsoftIdentityConsentAndConditionalAccessHandler)
        {
            this.apiServerHttpClient = apiServerHttpClient;
            this.microsoftIdentityConsentAndConditionalAccessHandler = microsoftIdentityConsentAndConditionalAccessHandler;            
        }

        public async Task<Entities.User?> Get()
        {            
            try
            {
                var result = await apiServerHttpClient.AuthorizedHttpClient.PostAsync($"api/UserProfile", null);
                var userEntity = await result.Content.ReadFromJsonAsync<Entities.User>();
                return userEntity;
            }
            catch (Exception ex)
            {
                microsoftIdentityConsentAndConditionalAccessHandler.HandleException(ex);
                return null;
            }
        }
                       
        private readonly IMicrosoftIdentityConsentAndConditionalAccessHandler microsoftIdentityConsentAndConditionalAccessHandler;
    }
}