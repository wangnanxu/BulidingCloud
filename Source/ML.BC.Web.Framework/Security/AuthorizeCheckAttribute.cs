using System;
using ML.BC.Infrastructure.Exceptions;
using ML.BC.Services;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using ML.BC.Infrastructure.Caching;
using ML.BC.Infrastructure.Common;

namespace ML.BC.Web.Framework.Security
{
    public class AuthorizeCheckAttribute : AuthorizeAttribute
    {
        private static ICacheManager cacheManager = ML.BC.Infrastructure.Ioc.GetService<ICacheManager>();
        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            var defaultResult = base.AuthorizeCore(httpContext);

            if (!defaultResult) return false;

            try
            {
                var sessionUser = System.Web.HttpContext.Current.Session.GetBCSession();
                if (sessionUser == null || sessionUser.User == null) return false;

                string cacheKey = string.Format("CheckUserIsLogin_{0}_{1}", sessionUser.User.UserID, sessionUser.User.Device);
                if (cacheManager.IsSet(cacheKey))
                {
                    defaultResult = defaultResult && cacheManager.Get<bool>(cacheKey);
                }
                else
                {
                    var accountService = ML.BC.Infrastructure.Ioc.GetService<IEnterpriseAccountService>();
                    var tempResult = accountService.CheckUserIsLogin(sessionUser.User.UserID, sessionUser.User.Device);
                    defaultResult = defaultResult && tempResult;
                    cacheManager.Set(cacheKey, tempResult, 60, false);
                }
                if (!defaultResult)
                {
                    sessionUser.Logout();
                }
            }
            catch (Exception)
            {
                return false;
            }
            return defaultResult;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!string.IsNullOrEmpty(filterContext.HttpContext.Request.Params[ConstantData.APPTOKENKey]))
            {
                throw new AppAuthorizeFailedException("用户验证失败，请重新登录。");
            }

            base.HandleUnauthorizedRequest(filterContext);
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
        }
    }
}
