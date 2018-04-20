using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Bugsnag.AspNet.Core;

namespace aspnetcore20_mvc
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Initialize Bugsnag to begin tracking errors. Only an api key is required, but here are some other helpful configuration details:
            services.AddBugsnag(configuration => {
                configuration.ApiKey = "665eea988511ad88d252916c3980e280";
                // We mark stacktrace lines as inProject if they come from namespaces included in your project namespaces.
                configuration.ProjectNamespaces = new[]{ "aspnetcore20_mvc" };
                // Project roots are used to strip file paths in each error reports stack trace in order to normalize them across builds.
                configuration.ProjectRoots = new[]{ @"/Users/reneebalmert/Documents/Code/notif_demos/dotnet/aspnetcore/bugsnag-dotnet/examples/" };
                configuration.AppType = "worker";
                configuration.AppVersion = "2.5.1";
                // Bugsnag can track the number of “sessions” that happen in your application, and calculate a crash rate for each release. This defaults to false.
                configuration.AutoCaptureSessions = true;
                // Metadata that will be attached to all error reports sent by the client.
                configuration.GlobalMetadata = new[] { new KeyValuePair<string, object>("company", new System.Collections.Generic.Dictionary<string, string> { 
                    { "department", "Westworld" },
                    { "name", "Delos Destinations, Inc." },
                    // because of the MetadataFilters, the below value will be redacted in your Bugsnag dashboard.
                    { "password", "frqhopsys76659" }
                   })
                };
                // Use this if you want to ensure you don’t send sensitive data such as passwords, and credit card numbers to our servers. Any keys in the metadata of the error report will have their values replaces with [FILTERED]
                configuration.MetadataFilters = new [] { "password", "creditcard" };
                //  defines which release stages bugsnag should report. e.g. ignore development errors.
                configuration.NotifyReleaseStages = new [] { "staging", "production" };
                configuration.ReleaseStage = "staging";
                // configuration.IgnoreClasses = new [] { typeof(Bugsnag.NotThatBadException) };
            });
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
