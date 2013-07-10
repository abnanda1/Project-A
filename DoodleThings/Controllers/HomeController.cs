using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DoodleThings.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Drawing()
        {
            return View();
        }
    }
}
