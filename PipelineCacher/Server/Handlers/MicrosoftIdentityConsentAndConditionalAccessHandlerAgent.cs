using Microsoft.Identity.Web;
using PipelineCacher.ClientServices;
using System;

namespace PipelineCacher.Server.Handlers
{
    public class MicrosoftIdentityConsentAndConditionalAccessHandlerAgent : IMicrosoftIdentityConsentAndConditionalAccessHandler
    {
        private readonly MicrosoftIdentityConsentAndConditionalAccessHandler microsoftIdentityConsentAndConditionalAccessHandler;

        public MicrosoftIdentityConsentAndConditionalAccessHandlerAgent(MicrosoftIdentityConsentAndConditionalAccessHandler microsoftIdentityConsentAndConditionalAccessHandler)
        {
            this.microsoftIdentityConsentAndConditionalAccessHandler = microsoftIdentityConsentAndConditionalAccessHandler;
        }        
        public void HandleException(Exception exception)
        {
            microsoftIdentityConsentAndConditionalAccessHandler.HandleException(exception);
        }
    }
}
