using System;
using System.Configuration;
using System.Web;
using System.Web.Mvc;

namespace ML.BC.Infrastructure
{
    public class CaptchaVerifyAttribute :ActionFilterAttribute
    {
        private readonly string _textError;

        public CaptchaVerifyAttribute()
        {
            _textError = "验证码错误";
        }

        public CaptchaVerifyAttribute(string textError)
        {
            _textError = textError;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = filterContext.Controller as Controller;
            var session = HttpContext.Current.Session[Config.SessionKey];
            var valid = true;
            if (session == null)
            {
                valid = false;
            }
            else
            {
                var text = controller.ValueProvider.GetValue(Config.InputName).AttemptedValue;
                valid = text.Equals(session.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            if (!valid)
            {
                var enableCaptchaVerify = false;
                bool.TryParse(ConfigurationManager.AppSettings["EnableCaptchaVerify"], out enableCaptchaVerify);
                if (enableCaptchaVerify)
                {
                  controller.ModelState.AddModelError(Config.InputName, _textError);
                }
            }
            HttpContext.Current.Session[Config.SessionKey] = null;
            base.OnActionExecuting(filterContext);
        }
    }
}
