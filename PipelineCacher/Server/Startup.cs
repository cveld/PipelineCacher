using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using PipelineCacher.Client.Services;
using PipelineCacher.ClientServices;
using PipelineCacher.Entities;
using PipelineCacher.Entities.Services;
using PipelineCacher.FakeUser;
using PipelineCacher.FakeUser.Handlers;
using PipelineCacher.Server.Handlers;
using PipelineCacher.Shared;
using PipelineCacher.Shared.Const;
using System.Linq;
//using Microsoft.EntityFrameworkCore.SqlServerDbContextOptionsExtensions;

namespace PipelineCacher.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //    .AddMicrosoftIdentityWebApi(Configuration.GetSection("AzureAd"));
            //services.AddMicrosoftIdentityWebApiAuthentication(Configuration, "AzureAd");
            var auth = services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme);
            //auth.AddMicrosoftIdentityWebApp(Configuration.GetSection("AzureAdClient"))
            //    .EnableTokenAcquisitionToCallDownstreamApi(new string[] { Const.APIScope })
            //    .AddInMemoryTokenCaches();
            //auth.AddMicrosoftIdentityWebApi(Configuration.GetSection("AzureAd"), JwtBearerDefaults.AuthenticationScheme);
            auth.AddScheme<FakeUserOptions, FakeUserAuthenticationHandler>(OpenIdConnectDefaults.AuthenticationScheme, null);
            auth.AddScheme<FakeUserOptions, FakeUserAuthenticationHandler>(JwtBearerDefaults.AuthenticationScheme, null);
            //services.AddAuthentication()
            //    .AddMicrosoftIdentityWebApi(Configuration.GetSection("AzureAd"),
            //                            "carl")
            //    .EnableTokenAcquisitionToCallDownstreamApi();
            //services.AddAuthorization(configure =>
            //{
            //    var result = configure.GetPolicy("carl");
            //});
            services.AddControllersWithViews().AddMicrosoftIdentityUI(); 
            services.AddRazorPages();
            //services.AddHttpClient();
            services.Configure<PipelineCacherConfig>(Configuration.GetSection(nameof(PipelineCacherConfig)));
            services.AddDbContext<PipelineCacherDbContext>(options => 
                options.UseSqlServer(Configuration.GetConnectionString("PipelineCacherDatabase")));
            //services.AddDatabaseDeveloperPageExceptionFilter();
            //services.AddSingleton<MigrationsUtility>();
            services.AddServerSideBlazor()
                .AddMicrosoftIdentityConsentHandler();

            //PipelineCacher.Client.Program.AddBlazorClientServices<ServerAuthorizationMessageHandler>(services, Configuration, null);
            PipelineCacher.Client.Program.AddBlazorClientServices<FakeUserMessageHandler>(services, Configuration, null);


            //services.AddMicrosoftIdentityWebAppAuthentication(Configuration.GetSection("AzureAdClient"));
            //services.AddTransient<ServerAuthorizationMessageHandler>();
            services.AddTransient<FakeUserMessageHandler>();
            services.AddTransient<UserProfileAgent>();
            services.AddTransient<IApiServerHttpClient, ApiServerBlazorServerHttpClient>();
            services.AddTransient<IMicrosoftIdentityConsentAndConditionalAccessHandler, MicrosoftIdentityConsentAndConditionalAccessHandlerAgent>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {           
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                //endpoints.MapFallbackToFile("index.html");
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
