using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Bugsnag;

namespace aspnetcore11_mvc.Controllers
{
  public class HomeController : Controller
  {
    private readonly IClient _client;
    private Dictionary<string, string> _messageLookup = new Dictionary<string, string>()
    {
      {
        "Contact", "Contact page"
      }
    };

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
      try
      {
        ViewData["Message"] = _messageLookup["Cotnact"];
      }
      catch (Exception e)
      {
        _client.Notify(e);
        ViewData["Message"] = "Your contact page.";
      }

      return View();
    }

    public IActionResult Error()
    {
      return View();
    }
  }
}
