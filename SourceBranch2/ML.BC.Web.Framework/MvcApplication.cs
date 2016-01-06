using ML.BC.Infrastructure.Common;
using System;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ML.BC.Web.Framework
{
    public class MvcApplication : HttpApplication
    {
        log4net.ILog log = log4net.LogManager.GetLogger("MvcApplication");

        private static bool _requestIntialized;

        protected void Application_Start()
        {
            GlobalFilters.Filters.Add(new HandleErrorAttribute());
            AreaRegistration.RegisterAllAreas();
            RegisterRoutes(RouteTable.Routes);
            //start schedule timer
            Schedule.Schedule.Instance.StartTimer();
            OnApplicationStart();
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            OnSessionStart();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (_requestIntialized == false)
            {
                _requestIntialized = true;
                OnFirstRequest();
            }
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            var isAuth = Request.IsAuthenticated;
            //Judge the request is whehter sent by App.
            if (isAuth && (!string.IsNullOrEmpty(Request.Params[ConstantData.APPTOKENKey]) || (!string.IsNullOrEmpty(Request.Params["DeviceId"]) && Request.Params["DeviceId"]!="PC")))
            {
                isAuth = false;
            }

            if (!isAuth)
            {
                HttpContext.Current.User = null;

                var token = Request.Params[ConstantData.APPTOKENKey];
                if (!string.IsNullOrEmpty(token))
                {
                    try
                    {
                        var ticket = System.Web.Security.FormsAuthentication.Decrypt(token);
                        HttpContext.Current.User = new System.Web.Security.RolePrincipal(new System.Web.Security.FormsIdentity(ticket));
                    }
                    catch
                    {
                        throw new ML.BC.Infrastructure.Exceptions.AppAuthorizeFailedException("用户登录消息丢失，请重新登录。");
                    }
                }
            }
        }

        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            var culture = new CultureInfo("zh-cn");
            Thread.CurrentThread.CurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();
            log.Error("Application_Error", exception);
            OnError(exception);
        }

        #region virtual members

        protected virtual void OnApplicationStart()
        {
        }

        protected virtual void OnFirstRequest()
        {
        }

        protected virtual void OnSessionStart()
        {
        }

        protected virtual void OnError(Exception exception)
        {
            //Logger.Error(exception);
        }

        protected virtual void RegisterRoutes(RouteCollection routes)
        {
        }

        #endregion

        private static WebApplicationEnum _webApplication;
        public static WebApplicationEnum WebApplication
        {
            get { return _webApplication; }
            protected set { _webApplication = value; }
        }
    }
}
