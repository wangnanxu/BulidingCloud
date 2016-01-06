﻿using System.Web.Mvc;

namespace ML.BC.EnterpriseWeb.Areas.ReportForm
{
    public class ReportFormAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "ReportForm";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ReportForm_default",
                "ReportForm/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
