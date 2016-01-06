using ML.BC.Infrastructure;
using ML.BC.Services;
using ML.BC.BCBackWeb.Areas.Account.Models;
using ML.BC.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ML.BC.Web.Framework;
using ML.BC.Infrastructure.Mvc;
using ML.BC.Infrastructure.Exceptions;
using System.Web.Security;

namespace ML.BC.BCBackWeb.Areas.Account.Controllers
{
    public class AccountController : BCControllerBase
    {
        public ActionResult Login()
        {
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

                var service = Ioc.GetService<IAccountService>();
                if (!service.CanLogin(model.Username, model.Password))
                {
              throw new KnownException("用户名/密码错误，登录失败");
                }
                Response.SetAuthCookie(model.Username, string.Empty, model.RememberMe);
                GetSession().Login(model.Username);
            });
            return result;
        }

        public RedirectToRouteResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            GetSession().Logout();
            return RedirectToRoute("Account_default", new { controller = "Account", action = "Login", id = UrlParameter.Optional });
        }

    }
}
