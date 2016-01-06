using System.Collections.Generic;
using System.Web.Mvc;
using ML.BC.Infrastructure;
using ML.BC.Web.Framework.Controllers;
using ML.BC.Services;
using ML.BC.Infrastructure.Mvc;
using ML.BC.Infrastructure.Exceptions;
using ML.BC.Web.Framework;
using ML.BC.BCBackWeb.Areas.Account.Models;
using ML.BC.BCBackWeb.Model;
using ML.BC.Web.Framework.Security;
namespace ML.BC.BCBackWeb.Areas.Account.Controllers
{
    [Authorize]
    public class EnterpriseAuthorizationManagementController : BCControllerBase
    {
        IEnterpriseRoleManagementService service;
        public EnterpriseAuthorizationManagementController()
        {
            service = Ioc.GetService<IEnterpriseRoleManagementService>();
        }
        public ActionResult AuthorizationIndex()
        {
            return View();
        }
         /// <summary>
        /// 获取企业角色的功能,授权
        /// </summary>
        /// <param name="RoleID"></param>
        /// <returns></returns>
        public ActionResult GetEnterpriseAuthorizationListByRoleID(int RoleID)
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
