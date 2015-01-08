//using Microsoft.AspNet.SignalR;

using System.Web.Mvc;

using System.Web.Routing;
using Zbang.Cloudents.Mobile.Extensions;

namespace Zbang.Cloudents.Mobile
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.LowercaseUrls = true;
            routes.AppendTrailingSlash = true;

            //var constraintsResolver = new DefaultInlineConstraintResolver();
            routes.MapMvcAttributeRoutes();


            routes.MapRoute("loginPage"
                , "account/login",
                new { controller = "Home", action = "IndexEmpty" },
                new { isget = new GetVsPostRouteConstraint("get") });

            routes.MapRoute("registerPage"
               , "account/register",
               new { controller = "Home", action = "IndexEmpty" },
               new { isget = new GetVsPostRouteConstraint("get") });

            routes.MapRoute("accountLink"
               , "account",
               new { controller = "Home", action = "IndexEmpty" });
            routes.MapRoute("libraryChoose"
               , "library/choose",
               new { controller = "Home", action = "IndexEmpty" });
            routes.MapRoute("dashboardLink"
               , "dashboard",
               new { controller = "Home", action = "IndexEmpty" });
            routes.MapRoute("box"
               , "box/my/{boxId}/{boxName}",
               new { controller = "Home", action = "IndexEmpty" });
            routes.MapRoute("course"
               , "course/{uniName}/{boxId}/{boxName}",
               new { controller = "Home", action = "IndexEmpty" });
            routes.MapRoute("search"
               , "search",
               new { controller = "Home", action = "IndexEmpty" });

            routes.MapRoute("Default", "{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );


        }
    }
}