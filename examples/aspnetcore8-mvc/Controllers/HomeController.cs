using System.Diagnostics;
using aspnetcore8_mvc.Models;
using Microsoft.AspNetCore.Mvc;

namespace aspnetcore8_mvc.Controllers
{
  public class HomeController : Controller
  {
    private readonly ILogger<HomeController> _logger;

    // The Bugsnag client (initialized in Program.cs) will be injected into your classes where you declare the IClient dependency.
    // This allows you to report handled exceptions within your controller.
    private readonly Bugsnag.IClient _bugsnag;

    public HomeController(ILogger<HomeController> logger, Bugsnag.IClient bugsnag)
    {
      _logger = logger;
      _bugsnag = bugsnag;
      var request = HttpContext?.Request;

      // A BeforeNotify callback lets you evaluate, modify, add and remove data before sending the error to bugsnag. The actions here will be applied to *all* errors, handled and unhandled.
      _bugsnag.BeforeNotify(report =>
      {
        // In order to correlate errors with customer reports, or to see a list of users who experienced each error, you can attach user data in your callback
        report.Event.User = new Bugsnag.Payload.User
        {
          Id = "006",
          Name = "Hedy Lamarr",
          Email = "h.lamarr@delos.com"
        };

        // This example makes some modifications that only apply to reports of error class "System.NotImplementedException".
        if (report.OriginalException is NotImplementedException)
        {
          report.Event.Context = "an-important-context";
        }

        // note that calling report.Ignore() will discard the error report.
      });
    }

    public IActionResult Index()
    {
      return View();
    }

    public IActionResult Contact()
    {
      // Report a handled exception
      try
      {
        throw new Exception("Handled exception");
      }
      catch (Exception ex)
      {
        _bugsnag.Notify(ex, report =>
        {
          // You can also customise individual error reports before sending to Bugsnag.
          report.Event.Metadata.Add("Don't worry", "I handled it");
        });
      }

      return View();
    }

    public IActionResult Problems()
    {
      // You can leave manual breadcrumbs via the Breadcrumbs property on the client object
      _bugsnag.Breadcrumbs.Leave("Here comes the exception...");

      // You can optionally attach a type and metadata to a breadcrumb.
      var metadata = new Dictionary<string, string> { { "message", "wait for it......" } };
      _bugsnag.Breadcrumbs.Leave("Here comes the exception...", Bugsnag.BreadcrumbType.Navigation, metadata);

      // Unhandled exceptions will be reported automatically
      throw new NotImplementedException("We have a problem");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }
}
