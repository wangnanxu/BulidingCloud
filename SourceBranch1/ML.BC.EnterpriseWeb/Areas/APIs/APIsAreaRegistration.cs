using System.Web.Mvc;

namespace ML.BC.EnterpriseWeb.Areas.APIs
{
    public class APIsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "APIs";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "APIs_default",
                "APIs/{controller}/{action}/{id}",
                new { controller = "Account", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "ML.BC.EnterpriseWeb.Areas.APIs.Controllers" }
            );
        }
    }
}
