using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ML.BC.Services;
using ML.BC.Web.Framework.Controllers;
using ML.BC.Infrastructure.Exceptions;
using ML.BC.EnterpriseWeb.Areas.Account.Models;
using ML.BC.EnterpriseWeb.Areas.Unit.Models;
using ML.BC.Infrastructure.Mvc;
using ML.BC.Services.Common;
using ML.BC.EnterpriseData.Common;
using ML.BC.EnterpriseWeb.Areas.ReportForm.Models;
using ML.BC.Web.Framework.Security;
namespace ML.BC.EnterpriseWeb.Areas.ReportForm.Controllers
{
    [AuthorizeCheck]
    public class ProjectSceneRectCountController : BCControllerBase
    {
        //
        // GET: /ReportForm/ProjectSceneRectCount/
        IRectificationStatistical service = null;
        public ProjectSceneRectCountController()
        {
            service = ML.BC.Infrastructure.Ioc.GetService<IRectificationStatistical>();
        }
        public ActionResult RectIndex()
        {
            return View();
        }
        [PermissionControlAttribute(Functions.Root_ReportManagement_ProjectSceneRectDataReport_ShowAll)]
        public ActionResult GetList(string userID,int? departmentID)
        {
            var result = new StandardJsonResult<ProjectSceneRectModel>();
            result.Try(() => {
                result.Value=new ProjectSceneRectModel();
                result.Value = service.GetRectificationStatistical(BCSession.User.EnterpriseID, departmentID, userID);
            });
            if (!result.Success)
            {
                result.Message = "获取数据失败！";
                result.Value = new ProjectSceneRectModel();
            }
            return result;
        }
    }
    
}
