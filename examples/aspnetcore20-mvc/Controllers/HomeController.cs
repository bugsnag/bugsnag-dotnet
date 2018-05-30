using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using aspnetcore20_mvc.Models;
using Bugsnag;

namespace aspnetcore20_mvc.Controllers
{
    public class HomeController : Microsoft.AspNetCore.Mvc.Controller
    {

        // The Bugsnag client (initialized in Startup.cs) will be injected into your classes where you declare the IClient dependency.
        // This allows you to report handled exceptions within your controller.
        private readonly Bugsnag.IClient _bugsnag;

        public HomeController(Bugsnag.IClient bugsnag)
        {
            _bugsnag = bugsnag;
            _bugsnag.BeforeNotify(report => {             
                if (report.OriginalException is System.NotImplementedException)
                {
                    report.Event.Metadata.Add("paying account", true);
                    report.Event.Context = "an-important-context";
                }
            // A BeforeNotify callback lets you evaluate, modify, add and remove data before sending the error to bugsnag. The actions here will be applied to *all* errors, handled and unhandled.
            _bugsnag.BeforeNotify(report => {
                // In order to correlate errors with customer reports, or to see a list of users who experienced each error, you can attach user data in your callback
                report.Event.User = new Bugsnag.Payload.User
                {
                    Id = "006",
                    Name = "Hedy Lamarr",
                    Email = "h.lamarr@delos.com"
                };
                //this example makes some modifications that only apply to reports of error class "System.NotImplementedException".
                if (report.OriginalException is System.NotImplementedException)
                {
                    report.Event.Metadata.Add("account", new Dictionary<string, object> { { "paying", true } });
                    report.Event.Context = "an-important-context";
                }

                // note that using report.Ignore() will cancel the entire error report.
            });
        }

        public IActionResult Index()
        {
            // creates an exception report, but sets the severity to "info". 
            _bugsnag.Notify(new System.Exception("Home page loaded"), report =>
            {
                // You can modify many attributes directly on a specific report before sending to Bugsnag.
                report.Event.Severity = Bugsnag.Severity.Info;
            });

            return View();
        }

        public IActionResult Problems()
        {
            _bugsnag.Breadcrumbs.Leave("Here comes the exception...");
            // You can leave manual breadcrumbs via the Breadcrumbs property on the client object.
            _bugsnag.Breadcrumbs.Leave("Something happened!");
            // You can optionally attach a type and metadata to a breadcrumb.
            var metadata = new Dictionary<string, string> { { "message", "wait for it......" } };
            _bugsnag.Breadcrumbs.Leave("Here comes the exception...", Bugsnag.BreadcrumbType.Navigation, metadata);

            // below deliberately throws an unhandled excpetion, which will automatically be reported by Bugsnag and crash the app.
            throw new NotImplementedException();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";
            try
            {
                throw new System.Exception("Error!");
            }
            catch (Exception ex)
            {
                throw new System.Exception("Error on contact page!");
            }
            catch (Exception ex)
            {
                // To report handled exceptions, make sure to declare the IClient dependency (above), then you can pass the exception object to your client for notification.
                _bugsnag.Notify(ex);
            }

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
