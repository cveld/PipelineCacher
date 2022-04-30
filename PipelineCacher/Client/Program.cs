using Blazored.Modal;
using Blazored.SessionStorage;
using BlazorState;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PipelineCacher.Client.Models;
using PipelineCacher.Client.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PipelineCacher.Client
{
    public class Program
    {
        public static IHttpClientBuilder AddHttpClientWithOptionalBaseAddress(IServiceCollection services, string name, string baseAddress)
        {
            IHttpClientBuilder result;
            if (baseAddress == null)
            {
                result = services.AddHttpClient(name);
            }
            else
            {
                result = services.AddHttpClient(name, client => client.BaseAddress = new Uri(baseAddress));
            }
            return result;

        }
        public static void AddBlazorClientServices<T>(IServiceCollection services, IConfiguration configuration, string baseAddress = null) where T : DelegatingHandler
        {
            AddHttpClientWithOptionalBaseAddress(services, HttpClientNames.Anonymous, baseAddress);
            var builder = AddHttpClientWithOptionalBaseAddress(services, HttpClientNames.Authorized, baseAddress).AddHttpMessageHandler<T>();

            // Supply HttpClient instances that include access tokens when making requests to the server project
            services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient(HttpClientNames.Authorized));
            

            services.AddBlazoredModal();
            services.AddBlazoredSessionStorage();
            services.AddBlazorContextMenu();
            services.AddOptions();
            services.AddBlazorState(
                (aOptions) =>
                aOptions.Assemblies =
                    new Assembly[]
                    {
                        typeof(Program).GetTypeInfo().Assembly,
                    }
            );
        }
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            //builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            //builder.Services.AddAuthorizationCore();
            builder.Services.AddMsalAuthentication(options =>
            {
                builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
                //options.ProviderOptions.DefaultAccessTokenScopes.Add("openid");
                options.ProviderOptions.DefaultAccessTokenScopes.Add("offline_access");
                options.ProviderOptions.DefaultAccessTokenScopes.Add("api://BlazorApp_Server/WeatherForecast.Read");
            });


            AddBlazorClientServices<BaseAddressAuthorizationMessageHandler>(builder.Services, builder.Configuration, builder.HostEnvironment.BaseAddress);

            // This does not do anything in the 5.0.3 version of the stack:
            // builder.Services.AddScoped<IClaimsTransformation, UserInfoClaims>();

            // Documented at https://docs.microsoft.com/en-us/aspnet/core/blazor/security/webassembly/graph-api?view=aspnetcore-5.0
            //builder.Services.AddApiAuthorization()
            //    .AddAccountClaimsPrincipalFactory<RolesClaimsPrincipalFactory>();

            await builder.Build().RunAsync();
        }
    }
}
