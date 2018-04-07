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
        private readonly Bugsnag.IClient _bugsnag;

        public HomeController(Bugsnag.IClient client)
        {
            _bugsnag = client;
            _bugsnag.BeforeNotify(report => {             
                if (report.OriginalException is System.NotImplementedException)
                {
                    report.Event.Metadata.Add("paying account", true);
                    report.Event.Context = "an-important-context";
                }
            });
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Problems()
        {
            _bugsnag.Breadcrumbs.Leave("Here comes the exception...");
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
