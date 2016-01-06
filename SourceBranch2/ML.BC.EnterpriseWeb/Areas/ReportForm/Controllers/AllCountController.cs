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
using ML.BC.EnterpriseWeb.Areas.ReportForm.Models;
using ML.BC.Services;
using ML.BC.EnterpriseData.Common;
using ML.BC.Web.Framework.Security;
using ML.BC.Infrastructure.Exceptions;
namespace ML.BC.EnterpriseWeb.Areas.ReportForm.Controllers
{
    [AuthorizeCheck]
    public class AllCountController : BCControllerBase
    {
        //
        // GET: /ReportForm/AllCount/
        private IInfoStatistics service;
        public AllCountController()
        {
            service=ML.BC.Infrastructure.Ioc.GetService<IInfoStatistics>();
        }
        public ActionResult AllIndex()
        {
            return View();
        }
        [PermissionControlAttribute(Functions.Root_ReportManagement_AllCountDataReport_ShowAll)]
        public ActionResult GetList()
        {
            var result = new StandardJsonResult<AllCountModel>();
            result.Try(() => {
                InfoStatisticsDto info = service.GetEnterpriseInfoStatistics(BCSession.User.EnterpriseID);
                if (info != null)
                {
                    result.Value = new AllCountModel() { 
                     ItemStatus=info.ItemStatus,
                     Pictures=info.Pictures,
                     Projects=info.Projects,
                     Scenes=info.Scenes,
                     Users=info.Users
                    };
                }
            });
            if (!result.Success)
                result.Value = new AllCountModel();
            return Json(result.Value, JsonRequestBehavior.AllowGet);
        }
    }
}
