using System;
using System.Web.Mvc;

namespace Web.MVCApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            if (ViewBag.Message != "DO NOT THROW")
                throw new FieldAccessException("Field Violation");
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}