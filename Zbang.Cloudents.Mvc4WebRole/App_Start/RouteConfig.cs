//using Microsoft.AspNet.SignalR;
using System.Web.Mvc;
using System.Web.Routing;

namespace Zbang.Cloudents.Mvc4WebRole.App_Start
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.LowercaseUrls = true;
            routes.AppendTrailingSlash = true;
            
            
            
          //  routes.MapRoute(
          //    "DownloadAll",
          //    "DownloadBox/{boxid}",
          //    new { controller = "Storage", action = "DownloadAllBoxItem", boxId = "boxid" }
          //);
            routes.MapMvcAttributeRoutes();
            //routes.MapRoute(
            //   "FileHandler",
            //   "D/{BoxUid}/{itemId}",
            //   new { controller = "Item", action = "Download", BoxUid = UrlParameter.Optional, ItemUid = UrlParameter.Optional });


            routes.MapRoute(
              "Sitemap",
              "sitemap.xml",
              new { controller = "Home", action = "SiteMap" }
              );

            routes.MapRoute("LibraryNode",
                "Library/{LibId}/{LibName}",
                new { controller = "Library", action = "Index", LibId = UrlParameter.Optional, LibName = UrlParameter.Optional },
                new { LibId = @"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$" });

            //routes.MapRoute(
            // "LibrarySearch",
            // "Library/Search/{query}",
            // new { controller = "Library", action = "Search", query = UrlParameter.Optional }
            // );

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

            //routes.MapRoute(
            //  "Box",
            //  "Box/{BoxUid}",
            //  new { controller = "Box", action = "Index", BoxUid = UrlParameter.Optional },
            //  new { isboxUid = new RouteUidConstraint() }
            //  );

            //routes.MapRoute(
            //    "box2",
            //    "box/{universityName}/{boxid}/{boxname}",
            //    new { controller = "Box", action = "Index" },
            //    new { boxid = @"\d+" }
            //    );

            routes.MapRoute(
              "Invite",
              "invite/{boxid}",
              new { controller = "Share", action = "Index", boxid = UrlParameter.Optional }
              );

            //routes.MapRoute(
            //    "User",
            //    "user/{userId}",
            //    new { controller = "User", action = "Index", userId = UrlParameter.Optional }
            //    );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );


        }

        //public static void RegisterApiRoutes(HttpConfiguration config)
        //{
        //    config.Routes.MapHttpRoute(
        //       name: "DefaultApi",
        //       routeTemplate: "api/{controller}/{action}/{id}",
        //       defaults: new { action = "Get", id = RouteParameter.Optional }
        //   );
        //}

        //public static void RegisterHubs()
        //{
        //    string connectionString = "Endpoint=sb://zbangcloudents.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=pwEE7wUSxPY/AkJu4LFXG3eVjZHvv+sKWI16ajW51qE=";
        //    //if (Microsoft.WindowsAzure.ServiceRuntime.RoleEnvironment.IsAvailable)
        //    //{
        //        GlobalHost.DependencyResolver.UseServiceBus(connectionString, "Cloudents");
        //    //}


        //    var config = new Microsoft.AspNet.SignalR.HubConfiguration();
        //    config.EnableJavaScriptProxies = false;
            
        //    if (!Microsoft.WindowsAzure.ServiceRuntime.RoleEnvironment.IsAvailable)
        //    {
        //        config.EnableDetailedErrors = true;
        //    }
        //    config.EnableCrossDomain = false;
            
            
        //    RouteTable.Routes.MapHubs("/RT", config);

        //}


    }


}