using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ML.BC.Web.ViewModels;
using ML.BC.Web.Framework.Controllers;
using ML.BC.Web.Framework.ViewModels;
using ML.BC.Infrastructure.Mvc;
using ML.BC.Infrastructure.Model;
using ML.BC.BCBackWeb.Areas.Account.Models;
using ML.BC.Services;
using ML.BC.EnterpriseData.Common;
using ML.BC.Web.Framework.Security;
using ML.BC.Infrastructure.Exceptions;
namespace ML.BC.BCBackWeb.Areas.Account.Controllers
{
    [Authorize]
    public class ReportCountController : Controller
    {
        //
        // GET: /Account/ReportCount/
         private IInfoStatistics service;
         public ReportCountController()
        {
            service=ML.BC.Infrastructure.Ioc.GetService<IInfoStatistics>();
        }
        public ActionResult CountIndex()
        {
            return View();
        }
        public ActionResult GetList()
        {
            var result = new StandardJsonResult<ReportCountModel>();
            result.Try(() =>
            {
                InfoStatisticsDto info = service.GetBackInfoStatistics();
                if (info != null)
                {
                    result.Value = new ReportCountModel()
                    {
                        ItemStatus = info.ItemStatus,
                        Pictures = info.Pictures,
                        Projects = info.Projects,
                        Scenes = info.Scenes,
                        Users = info.Users
                    };
                }
            });
            if (!result.Success)
                result.Value = new ReportCountModel();
            return Json(result.Value, JsonRequestBehavior.AllowGet);
        }
    }
}
