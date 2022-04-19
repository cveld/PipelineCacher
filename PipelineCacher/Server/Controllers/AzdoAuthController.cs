using System;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using PipelineCacher.Entities;
using PipelineCacher.Server.Utilities;
using Microsoft.Extensions.Configuration;
using PipelineCacher.Shared.Models;
using Microsoft.AspNetCore.Authorization;

namespace PipelineCacher.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]

    public class AzdoAuthController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly HttpClient httpClient;
        private readonly PipelineCacherDbContext pipelineCacherDbContext;

        public AzdoAuthController(IConfiguration configuration, HttpClient httpClient, PipelineCacherDbContext pipelineCacherDbContext)
        {
            this.configuration = configuration;
            this.httpClient = httpClient;
            this.pipelineCacherDbContext = pipelineCacherDbContext;
        }
        /// <summary>
        /// Callback action. Invoked after the user has authorized the app.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AzdoAuthResult> RetrieveToken([FromBody]String code)
        {
            String error = null;
            
            // Exchange the auth code for an access token and refresh token
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, configuration["Azdo:TokenUrl"]);
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            Dictionary<String, String> form = new Dictionary<String, String>()
            {
                { "client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer" },
                { "client_assertion", configuration["Azdo:ClientAppSecret"] },
                { "grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer" },
                { "assertion", code },
                { "redirect_uri", configuration["Azdo:CallbackUrl"] }
            };
            requestMessage.Content = new FormUrlEncodedContent(form);

            HttpResponseMessage responseMessage = await httpClient.SendAsync(requestMessage);

            if (responseMessage.IsSuccessStatusCode)
            {
                String body = await responseMessage.Content.ReadAsStringAsync();

                AzdoToken tokenModel = new AzdoToken();                
                JsonConvert.PopulateObject(body, tokenModel);
                pipelineCacherDbContext.AzdoTokens.Add(tokenModel);
                await pipelineCacherDbContext.SaveChangesAsync();

                return new AzdoAuthResult
                {
                    AzdoTokenProjection = new AzdoTokenProjection
                    {
                        Id = tokenModel.Id,
                        ExpiresIn = tokenModel.ExpiresIn
                    }
                };
            }
            else
            {
                error = responseMessage.ReasonPhrase;
            }

            return new AzdoAuthResult
            {
                Error = error
            };
        }
      
        /// <summary>
        /// Gets a new access
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public async Task<string> RefreshToken(string refreshToken)
        {
            String error = null;
            if (!String.IsNullOrEmpty(refreshToken))
            {
                // Form the request to exchange an auth code for an access token and refresh token
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, configuration["Azdo:TokenUrl"]);
                requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                Dictionary<String, String> form = new Dictionary<String, String>()
                {
                    { "client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer" },
                    { "client_assertion", configuration["Azdo:ClientAppSecret"] },
                    { "grant_type", "refresh_token" },
                    { "assertion", refreshToken },
                    { "redirect_uri", configuration["Azdo:CallbackUrl"] }
                };
                requestMessage.Content = new FormUrlEncodedContent(form);

                // Make the request to exchange the auth code for an access token (and refresh token)
                HttpResponseMessage responseMessage = await httpClient.SendAsync(requestMessage);

                if (responseMessage.IsSuccessStatusCode)
                {
                    // Handle successful request
                    String body = await responseMessage.Content.ReadAsStringAsync();
                    var token = JObject.Parse(body).ToObject<AzdoToken>();
                    return Serializers.ObjectToJson(token);
                }
                else
                {
                    error = responseMessage.ReasonPhrase;
                }
            }
            else
            {
                error = "Invalid refresh token";
            }
            return error;
        }       
    }
}
