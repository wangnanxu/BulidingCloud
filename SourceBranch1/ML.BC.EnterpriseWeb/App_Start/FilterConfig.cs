﻿using System.Web;
using System.Web.Mvc;

namespace ML.BC.EnterpriseWeb
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}