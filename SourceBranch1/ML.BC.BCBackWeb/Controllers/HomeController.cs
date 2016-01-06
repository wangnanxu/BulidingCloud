using ML.BC.Web.Framework.Controllers;
using ML.BC.Web.Framework.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ML.BC.BCBackWeb.Controllers
{
   [Authorize]
    public class HomeController:BCControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SubHome()
        {
            return View("SubHomePage");
        }
        public ActionResult Construction()
        {
            return View("Construction");
        }
    }
}