﻿using System.Web.Mvc;

namespace ML.BC.BCBackWeb.Areas.System
{
    public class SystemAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "System";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "System_default",
                "System/{controller}/{action}/{id}",
                new { action = "SystemLogIndex", id = UrlParameter.Optional }
            );
        }
    }
}
