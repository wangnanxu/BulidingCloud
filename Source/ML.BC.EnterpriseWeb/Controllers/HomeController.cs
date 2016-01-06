using ML.BC.Infrastructure;
using ML.BC.Infrastructure.Mvc;
using ML.BC.Services;
using ML.BC.Web.Framework;
using ML.BC.Web.Framework.Controllers;
using ML.BC.Web.Framework.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ML.BC.EnterpriseWebControllers
{
    [AuthorizeCheck]
    public class HomeController : BCControllerBase
    {
        public ActionResult Index()
        {

            var session = GetSession();
            if (session == null || session.User == null) ViewBag.depart = "请登录";
            else
            {
                if (session.User.DepartmentID == null)
                {
                    ViewBag.depart = "无部门";
                }
                else
                {
                    ViewBag.depart = Ioc.GetService<IEnterpriseDepartmentManagementService>().GetDepartmentNameById(session.User.DepartmentID ?? 0);
                }
            }
            return View(ViewBag);
        }
        public ActionResult SubHome()
        {
            return View("SubHomePage");
        }
        public ActionResult Construction()
        {
            return View("Construction");
        }
        public ActionResult KeepSession(string timeSpan)
        {
            var result = new StandardJsonResult();
            result.Try(() => {
                var service = Ioc.GetService<IEnterpriseAccountService>();
                service.RefreshUserLoginState(BCSession.User.UserID, BCSession.User.Device);
            });
            return result;
        }
        public ActionResult GetMessageCount()
        {
            var result = new StandardJsonResult<int>();
            result.Try(() => {
                var services=ML.BC.Infrastructure.Ioc.GetService<IChatMessageService>();
                result.Value = services.GetMessageCount(BCSession.User.UserID);
            });
            return result;
        }
    }
}