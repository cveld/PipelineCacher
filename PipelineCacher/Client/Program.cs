using Blazored.Modal;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PipelineCacher.Client.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PipelineCacher.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddHttpClient("blazorauth1.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            // Supply HttpClient instances that include access tokens when making requests to the server project
            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("blazorauth1.ServerAPI"));

            //builder.Services.AddAuthorizationCore();
            builder.Services.AddMsalAuthentication(options =>
            {                
                builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
                options.ProviderOptions.DefaultAccessTokenScopes.Add("openid");
                options.ProviderOptions.DefaultAccessTokenScopes.Add("offline_access");
                options.ProviderOptions.DefaultAccessTokenScopes.Add("api://BlazorApp_Server/WeatherForecast.Read");
            });

            

            //builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddBlazoredModal();
            builder.Services.AddBlazorContextMenu();
            builder.Services.AddOptions();
            builder.Services.AddSingleton<UserState>();

            // This does not do anything in the 5.0.3 version of the stack:
            // builder.Services.AddScoped<IClaimsTransformation, UserInfoClaims>();

            // Documented at https://docs.microsoft.com/en-us/aspnet/core/blazor/security/webassembly/graph-api?view=aspnetcore-5.0
            builder.Services.AddApiAuthorization()
                .AddAccountClaimsPrincipalFactory<RolesClaimsPrincipalFactory>();

            await builder.Build().RunAsync();
        }
    }
}
