using System.Web.Mvc;

namespace ML.BC.EnterpriseWeb.Areas.Specification
{
    public class SpecificationAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Specification";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Specification_default",
                "Specification/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "ML.BC.EnterpriseWeb.Areas.Specification.Controllers" }
            );
        }
    }
}
