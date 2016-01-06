using System.Web.Mvc;

namespace ML.BC.BCBackWeb.Areas.Account
{
    public class AccountAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Account";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Account_default",
                "Account/{controller}/{action}/{id}",
                //"Account/{action}/{id}",
                new {controller="Account", action = "Login", id = UrlParameter.Optional }
            );
        }
    }
}
