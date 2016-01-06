using System.Web.Mvc;

namespace ML.BC.BCBackWeb.Areas.Specification
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
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
