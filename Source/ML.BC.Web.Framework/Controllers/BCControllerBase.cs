using ML.BC.Infrastructure.Exceptions;
using ML.BC.Infrastructure.Mvc;
using ML.BC.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using ML.BC.Infrastructure;
using ML.BC.Services;

namespace ML.BC.Web.Framework.Controllers
{
    public class BCControllerBase : Controller
    {
        private IPermissionControlService _permissionControlService = null;
        private IPermissionControlService PermissionControlService
        {
            get
            {
                _permissionControlService = _permissionControlService ?? ML.BC.Infrastructure.Ioc.GetService<IPermissionControlService>();
                return _permissionControlService;
            }
        }
        protected virtual IBCSession BCSession
        {
            get
            {
                return System.Web.HttpContext.Current.Session.GetBCSession();
            }
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;
            log4net.ILog log = log4net.LogManager.GetLogger("BCControllerBase");
            log.Error(filterContext.Controller+"\r\n"+filterContext.Exception);
            Response.AddHeader("Access-Control-Allow-Origin", "*");
            string error;
            if (filterContext.Exception is AppAuthorizeFailedException)
            {
                error = filterContext.Exception.Message;
                filterContext.Result = Json(new { LoginFailed = true, Message = error }, JsonRequestBehavior.AllowGet);
                return; 
            }
            else if (filterContext.Exception is KnownException)
            {
                error = filterContext.Exception.Message;
            }
            else
            {
                error = "服务器未知错误，请重试。如果该问题一直存在，请联系管理员。感谢您的支持。";
#if DEBUG
                error = filterContext.Exception.GetAllMessages();
#endif
            }

            if (Request.IsAjaxRequest())
            {
                var result = new StandardJsonResult();
                result.Fail(error);
                filterContext.Result = result;
            }
            else
            {
                var model = new LayoutViewModel();
                model.Error = error;
                filterContext.Result = this.View(this.GetErrorViewPath(), model);
            }
        }

        protected virtual string GetErrorViewPath()
        {
            return "~/Views/Shared/Error.cshtml";
        }

        protected virtual IBCSession GetSession()
        {
            return System.Web.HttpContext.Current.Session.GetBCSession();
        }

        protected bool HasFunction(string functionID)
        {
            return PermissionControlService.HasPermission(BCSession.User, new string[] { functionID });
        }
        protected bool HasFunction(string[] functionIDs)
        {
            return PermissionControlService.HasPermission(BCSession.User, functionIDs);
        }

    }
}
