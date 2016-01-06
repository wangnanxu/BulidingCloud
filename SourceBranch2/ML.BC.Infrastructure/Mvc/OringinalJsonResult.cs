using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ML.BC.Infrastructure.Mvc
{
    public class OringinalJsonResult<T> : ActionResult where T : class
    {
        public T Value { get; set; }
        public override void ExecuteResult(ControllerContext context)
        {
            WriteToResponse(context.HttpContext);
        }
        public void WriteToResponse(HttpContextBase httpContext)
        {
            var response = httpContext.Response;

                response.ContentType = "application/json";
            
            bool BrIdhaveIE = false;
            if (!BrIdhaveIE)
            {
                if (!String.IsNullOrEmpty(httpContext.Request.Browser.Browser))
                {

                    BrIdhaveIE = httpContext.Request.Browser.Browser.ToLower().Contains("ie");
                }
            }

            if (!BrIdhaveIE)
            {
                BrIdhaveIE = httpContext.Request.Browser.ActiveXControls;
            }

            if (!String.IsNullOrEmpty(httpContext.Request.Browser.Id))
            {
                BrIdhaveIE = httpContext.Request.Browser.Id.ToLower().Contains("ie");
            }

            if (BrIdhaveIE)
            {
                if (response.ContentType == "application/json")
                {
                    response.ContentType = "text/html";
                }
            }

            response.ContentEncoding = Encoding.UTF8;
            response.Write(Serializer.ToJson(this.Value));
        }
    }
}
