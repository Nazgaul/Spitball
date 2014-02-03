﻿using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace Zbang.Zbox.WebApi
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            WebApiFilterConfig.RegisterGlobalFilters(GlobalConfiguration.Configuration.Filters);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            Bootstrapper.Initialise();
        }
    }
}