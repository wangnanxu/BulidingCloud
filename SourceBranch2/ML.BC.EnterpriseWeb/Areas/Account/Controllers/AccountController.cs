using ML.BC.Infrastructure;
using ML.BC.Services;
using ML.BC.EnterpriseWeb.Areas.Account.Models;
using ML.BC.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ML.BC.Web.Framework;
using ML.BC.Infrastructure.Exceptions;
using ML.BC.Infrastructure.Mvc;
using System.Web.Security;
using ML.BC.Web.Framework.ViewModels;
using ML.BC.Services.Account.Dtos;
using ML.BC.Web.Framework.Security;
using ML.BC.EnterpriseData.Common;

namespace ML.BC.EnterpriseWeb.Areas.Account.Controllers
{

    public class AccountController : BCControllerBase
    {
        IEnterpriseAccountService service;
        public AccountController()
        {
            service = Ioc.GetService<IEnterpriseAccountService>();
        }
        public ActionResult Login()
        {
            FormsAuthentication.SignOut();
            GetSession().Logout();
            Session.Clear();
            Session.Abandon();
            return View(new LoginViewModel());
        }

        [HttpPost, CaptchaVerify]
        public ActionResult Login(LoginViewModel model)
        {
            var result = new StandardJsonResult();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }

                var service = Ioc.GetService<IEnterpriseAccountService>();
                if (!service.CanLogin(model.Username, model.Password))
                {
                    throw new KnownException("用户名/密码错误，登录失败");
                }
                BCSession.Login(model.Username);
            });
            return result;
        }

        [AuthorizeCheck]
        public RedirectToRouteResult Logout()
        {
            FormsAuthentication.SignOut();
            try
            {
                GetSession().Logout();
            }
            finally
            {
                Session.Clear();
                Session.Abandon();                
            }
            return RedirectToRoute("Account_default", new { controller = "Account", action = "Login", id = UrlParameter.Optional });
        }

        #region 用户登录状态,暂时放在此处，以后要移到OnlineUserController中
        [AuthorizeCheck]
        [PermissionControlAttribute(Functions.Root_SystemSetting_UserLoginStateManagement)]
        public ActionResult UserLoginStateIndex()
        {
            return View();
        }

        [AuthorizeCheck]
        [PermissionControlAttribute(Functions.Root_SystemSetting_UserLoginStateManagement_List)]
        public ActionResult GetUserLoginStateList(UserLoginStateViewModel model)
        {

            var result = new StandardJsonResult<GridDataModelBase<UserLoginStateDto>>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }

                int count = 0;
                bool showall = false;
                if (HasFunction(Functions.Root_SystemSetting_UserLoginStateManagement_ShowAll))
                {
                    showall = true;
                }

                SessionUserDto _sessionUserDto = new SessionUserDto()
                {
                    DepartmentID = BCSession.User.DepartmentID,
                    EnterpriseID = BCSession.User.EnterpriseID,
                    UserID = BCSession.User.UserID
                };

                List<UserLoginStateDto> listdto = service.GetUserLoginStateList(_sessionUserDto, showall, model.UserName, model.page, model.rows, out count);
                result.Value = new GridDataModelBase<UserLoginStateDto>()
                {
                    rows = listdto,
                    total = count
                };

            });
            if (result.Success == false) result.Value = new GridDataModelBase<UserLoginStateDto>();
            return Json(result.Value, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeCheck]
        [PermissionControlAttribute(Functions.Root_SystemSetting_UserLoginStateManagement_Delete)]
        public ActionResult DeleteUserLoginState(UserLoginStateDto model)
        {

            var result = new StandardJsonResult<string>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                bool i = false;
                if (model.UserLoginStateID > 0)
                {
                    i = service.DeleteUserLoginState(model.UserLoginStateID, "管理员强制登出");
                }
                else if (model.UserID != "")
                {
                    i = service.DeleteUserLoginState(model.UserID, model.Device, "管理员强制登出");
                }

                result.Value = i + "";
            });
            return result;
        }
        #endregion
    }
}
