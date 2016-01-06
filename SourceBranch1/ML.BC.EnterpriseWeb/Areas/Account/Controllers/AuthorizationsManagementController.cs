using System.Collections.Generic;
using System.Web.Mvc;
using ML.BC.Infrastructure;
using ML.BC.Web.Framework.Controllers;
using ML.BC.Services;
using ML.BC.Infrastructure.Mvc;
using ML.BC.Infrastructure.Exceptions;
using ML.BC.Web.Framework;
using ML.BC.EnterpriseWeb.Areas.Account.Models;
using ML.BC.Web.Framework.Security;
using ML.BC.EnterpriseData.Common;
using ML.BC.Web.Framework.ViewModels;
namespace ML.BC.EnterpriseWeb.Areas.Account.Controllers
{
    /// <summary>
    /// 企业后端 角色功能,授权`
    /// </summary>
    [AuthorizeCheck]
    public class AuthorizationManagementController : BCControllerBase
    {
        IEnterpriseRoleManagementService service;
        public AuthorizationManagementController()
        {
            service = Ioc.GetService<IEnterpriseRoleManagementService>();
        }
        public ActionResult AuthorizationIndex()
        {
            return View();
        }

        /// <summary>
        /// 获取角色的功能列表
        /// </summary>
        /// <param name="RoleID"></param>
        /// <returns></returns>
        public ActionResult GetAuthorizationListByRoleID(int RoleID)
        {
            var result = new StandardJsonResult<List<string>>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }

                List<string> list = service.GetFunctions(RoleID);
                result.Value = list != null ? list : new List<string>();
            });
            if (result.Success == false) result.Value = new List<string>();
            return Json(result.Value, JsonRequestBehavior.AllowGet);
        }


    }
}
