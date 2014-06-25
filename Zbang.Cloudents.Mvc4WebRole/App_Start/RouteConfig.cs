//using Microsoft.AspNet.SignalR;

using System.Web.Mvc;
using System.Web.Routing;

namespace Zbang.Cloudents.Mvc4WebRole
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.LowercaseUrls = true;
            routes.AppendTrailingSlash = true;
            
            routes.MapMvcAttributeRoutes();

            routes.MapRoute("AccountLanguage",
                "account/{lang}",
                new {controller = "Account", action = "Index", lang = UrlParameter.Optional},
                new {lang = "^[A-Za-z]{2}-[A-Za-z]{2}$"});
            

            routes.MapRoute(
              "Sitemap",
              "sitemap.xml",
              new { controller = "Home", action = "SiteMap" , index= UrlParameter.Optional }
              );

            routes.MapRoute(
              "Sitemap1",
              "sitemap-{index}.xml",
              new { controller = "Home", action = "SiteMap", index = UrlParameter.Optional }
              );

            routes.MapRoute("LibraryNode",
                "Library/{LibId}/{LibName}",
                new { controller = "Library", action = "Index", LibId = UrlParameter.Optional, LibName = UrlParameter.Optional },
                new { LibId = @"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$" });

            routes.MapRoute(
              "DashboardSearch",
              "Dashboard/Search/{query}",
              new { controller = "Dashboard", action = "Search", query = UrlParameter.Optional }
              );

            routes.MapRoute(
              "BoxSetting",
              "Box/Settings/{BoxUid}",
              new { controller = "Box", action = "Settings", BoxUid = UrlParameter.Optional }
              );

            routes.MapRoute(
              "Invite",
              "invite/{boxid}",
              new { controller = "Share", action = "Index", boxid = UrlParameter.Optional }
              );
            

            routes.MapRoute("Default", "{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );


        }
    }
}