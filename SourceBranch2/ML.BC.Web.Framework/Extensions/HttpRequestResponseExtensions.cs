using System;
using System.Web;
using System.Web.Security;

namespace ML.BC.Web.Framework
{
    public static class HttpRequestResponseExtensions
    {
        public static string SetAuthCookie(this HttpResponseBase response, string username,string device, bool remember30Days)
        {
            var ticket = new FormsAuthenticationTicket(1, username, DateTime.Now, DateTime.Now.AddDays(30),
                remember30Days,
                device);
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName);
            cookie.Value = FormsAuthentication.Encrypt(ticket);
            if (remember30Days)
            {
                cookie.Expires = DateTime.Now.AddDays(30);
            }
            response.Cookies.Add(cookie);
            return cookie.Value;
        }
        public static string SetAuthCookie(this HttpResponse response, string username, string device, bool remember30Days)
        {
            var ticket = new FormsAuthenticationTicket(1, username, DateTime.Now, DateTime.Now.AddDays(30),
                remember30Days,
                device);
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName);
            cookie.Value = FormsAuthentication.Encrypt(ticket);
            if (remember30Days)
            {
                cookie.Expires = DateTime.Now.AddDays(30);
            }
            response.Cookies.Add(cookie);
            return cookie.Value;
        }
        public static string GetRequestIP(this HttpRequest request)
        {
            string result = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(result))
            {
                result = request.ServerVariables["REMOTE_ADDR"];
            }
            if (string.IsNullOrEmpty(result))
            {
                result = request.UserHostAddress;
            }
            if (string.IsNullOrEmpty(result))
            {
                result = "0.0.0.0";
            }
            return result;
        }
        /// <summary>
        /// 获取移动端的设备信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetRequestDevice(this HttpRequest request)
        {
            var deviceId = request.Params["DeviceID"];
            if (string.IsNullOrEmpty(deviceId))
            {
                deviceId = "PC"; 
            }
            return deviceId;
        }
        public static bool GetRequestRememberMe(this HttpRequest request)
        {
            bool RememberMe = false;
            bool.TryParse(HttpContext.Current.Request["RememberMe"], out RememberMe);
            return RememberMe;
        }

    }
}