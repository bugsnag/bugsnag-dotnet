using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace aspnet462_mvc_webapi.Controllers
{
  public class HomeController : Controller
  {
    public ActionResult Index()
    {
      return View();
    }

    public ActionResult About()
    {
      try
      {
        throw new Exception("Handled MVC exception");
      }
      catch (Exception ex)
      {
        Bugsnag.AspNet.Client.Current.Notify(ex);
      }

      ViewBag.Message = "Your application description page.";

      return View();
    }

    public ActionResult Contact()
    {
      ViewBag.Message = "Your contact page.";

      return View();
    }

    public ActionResult Crash()
    {
      throw new NotImplementedException("Unhandled MVC exception");
    }
  }
}