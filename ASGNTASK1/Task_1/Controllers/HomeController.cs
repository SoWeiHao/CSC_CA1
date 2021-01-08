using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Task_1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "CSC Assignment Task 1";

            return View();
        }
        public ActionResult WeatherDetailsCSharp()
        {
            ViewBag.Title = "CSC Assignment Task 1";

            return View();
        }
    }
}
