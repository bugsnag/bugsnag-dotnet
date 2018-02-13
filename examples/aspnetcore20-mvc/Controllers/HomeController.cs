using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using aspnetcore20_mvc.Models;
using Bugsnag.AspNet.Core;

namespace aspnetcore20_mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly IClient _client;

        public HomeController(IClient client)
        {
          _client = client;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            _client.Breadcrumbs.Leave("Here comes the exception...");
            throw new NotImplementedException();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
