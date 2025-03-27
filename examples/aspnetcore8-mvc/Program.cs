using Bugsnag.AspNet.Core;

var builder = WebApplication.CreateBuilder(args);

// Initialize Bugsnag to begin tracking errors. Only an api key is required, but here are some other helpful configuration details:
builder.Services.AddBugsnag(configuration =>
{
  // Replace this with your own API key.
  configuration.ApiKey = "YOUR_API_KEY";

  // We mark stacktrace lines as inProject if they come from namespaces included in your project namespaces.
  configuration.ProjectNamespaces = new[] { "aspnetcore8_mvc" };

  // Project roots are used to strip file paths in each error reports stack trace in order to normalize them across builds.
  // configuration.ProjectRoots = new[]{ @"/Users/bgates/bugsnag-dotnet/examples/" };
  configuration.AppType = "worker";
  configuration.AppVersion = "2.5.1";

  // Bugsnag can track the number of “sessions” that happen in your application, and calculate a crash rate for each release. This defaults to false.
  configuration.AutoCaptureSessions = true;

  // Metadata that will be attached to all error reports sent by the client.
  configuration.GlobalMetadata = new[] { new KeyValuePair<string, object>("company", new Dictionary<string, string> {
      { "department", "Westworld" },
      { "name", "Delos Destinations, Inc." },
      // these values will be redacted from the report due to the MetadataFilters configuration.
      { "password", "frqhopsys76659" },
      { "creditcard", "1234-5678-1234-5678" }
      })
  };

  // Use MetadataFilters to remove sensitive data such as passwords, and credit card numbers from error reports.
  // Any matching keys in the metadata of the report will have their values replaces with [FILTERED]. "password" and "Authorization" are filtered by default.
  configuration.MetadataFilters = new[] { "password", "creditcard" };

  // Use NotifyReleaseStages to disable error reporting in certain environments, e.g. ignore development errors.
  configuration.NotifyReleaseStages = new[] { "staging", "production" };
  configuration.ReleaseStage = "staging";

  // Use IgnoreClasses to define exception types that should not be reported.
  // configuration.IgnoreClasses = new [] { typeof(Bugsnag.NotThatBadException) };
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();

  // Bugsnag automatically reports exceptions that the exception handler middleware catches.
  app.UseExceptionHandler("/Home/Error");
}
else
{
  // Bugsnag automatically reports exceptions that the developer exception page middleware catches.
  app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
