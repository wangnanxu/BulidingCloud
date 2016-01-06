using System.Web.Mvc;

namespace ML.BC.EnterpriseWeb.Areas.Scene
{
    public class SceneAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Scene";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Scene_default",
                "Scene/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
