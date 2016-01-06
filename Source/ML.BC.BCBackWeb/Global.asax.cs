using ML.BC.Infrastructure;
using ML.BC.Services;
using ML.BC.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ML.BC.BCBackWeb
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class BCMvcApplication : MvcApplication
    {
        protected override void OnApplicationStart()
        {
            base.OnApplicationStart();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            WebApplication = WebApplicationEnum.BCBackWeb;

            //Ioc.RegisterInheritedTypes(typeof(IServiceBase).Assembly, typeof(IServiceBase));
        }
    }
}