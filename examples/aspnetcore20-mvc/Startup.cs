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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddBugsnag(configuration => {
                configuration.ApiKey = "665eea988511ad88d252916c3980e280";
                // We mark stacktrace lines as inProject if they come from namespaces included in your project namespaces.
                configuration.ProjectNamespaces = new[]{ "aspnetcore20_mvc" };
                // Project roots are used to strip file paths in each error reports stack trace in order to normalize them across builds.
                configuration.ProjectRoots = new[]{ @"/Users/reneebalmert/Documents/Code/notif_demos/dotnet/aspnetcore/bugsnag-dotnet/examples/" };
                configuration.AppType = "worker";
                configuration.AppVersion = "2.5.1";
                configuration.AutoCaptureSessions = true;
                // Metadata that will be attached to all error reports sent by the client.
                configuration.GlobalMetadata = new[] { new KeyValuePair<string, object>(
                  "account",
                  new System.Collections.Generic.Dictionary<string, string>
                    { { "password", "password123" } }
                  )
                };
                // Use this if you want to ensure you don’t send sensitive data such as passwords, and credit card numbers to our servers. Any keys in the metadata of the error report will have their values replaces with [FILTERED]
                configuration.MetadataFilters = new [] { "password", "creditcard" };
                configuration.NotifyReleaseStages = new [] { "staging", "production" };
                configuration.ReleaseStage = "staging";
                // configuration.IgnoreClasses = new [] { typeof(Bugsnag.NotThatBadException), typeof(System.NotImplementedException) };
            });
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
