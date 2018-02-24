using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace aspnet45_mvc_webapi.Controllers
{
  public class HomeController : Controller
  {
    public ActionResult Index()
    {
      ViewBag.Title = "Home Page";

      return View();
    }

    public ActionResult Crash()
    {
      throw new NotImplementedException("oh noes");
    }
  }
}
