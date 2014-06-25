﻿//using Microsoft.AspNet.SignalR;

using System.Web.Mvc;
using System.Web.Mvc.Routing;
using System.Web.Mvc.Routing.Constraints;
using System.Web.Routing;
using Zbang.Cloudents.Mvc4WebRole.Helpers;

namespace Zbang.Cloudents.Mvc4WebRole
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.LowercaseUrls = true;
            routes.AppendTrailingSlash = true;

            var constraintsResolver = new DefaultInlineConstraintResolver();
            constraintsResolver.ConstraintMap.Add("desktop", typeof (DesktopConstraint));
            routes.MapMvcAttributeRoutes(constraintsResolver);


            routes.MapRoute("AccountLanguage",
                "account/{lang}",
                new { controller = "Account", action = "Index", lang = UrlParameter.Optional },
                new { lang = "^[A-Za-z]{2}-[A-Za-z]{2}$" });


            routes.MapRoute("DashboardDesktop",
                "dashboard",
                new { controller = "Home", action = "Index" },
                new { isDesktop = new DesktopConstraint() }
            );

          

            #region Box
            routes.MapRoute("PrivateBoxDesktop",
                  "box/my/{boxId}/{boxName}",
                  new { controller = "Home", action = "Index" },
                  new { isDesktop = new DesktopConstraint(), boxId = new LongRouteConstraint() }
              );
            routes.MapRoute("CourseBoxDesktop",
              "course/{universityName}/{boxId}/{boxName}",
              new { controller = "Home", action = "Index" },
              new { isDesktop = new DesktopConstraint(), boxId = new LongRouteConstraint() }
          );

            routes.MapRoute("PrivateBox",
            "box/my/{boxId}/{boxName}",
            new { controller = "Box", action = "Index" },
            new { boxId = new LongRouteConstraint() }
        );
            routes.MapRoute("CourseBox",
              "course/{universityName}/{boxId}/{boxName}",
              new { controller = "Box", action = "Index" },
              new { boxId = new LongRouteConstraint() }
          ); 
            #endregion

            routes.MapRoute(
              "Sitemap",
              "sitemap.xml",
              new { controller = "Home", action = "SiteMap", index = UrlParameter.Optional }
              );

            routes.MapRoute(
              "Sitemap1",
              "sitemap-{index}.xml",
              new { controller = "Home", action = "SiteMap", index = UrlParameter.Optional }
              );

            #region library
            routes.MapRoute("LibraryDesktop",
                "library/{LibId}/{LibName}",
                new { controller = "Home", action = "Index" },
                new { isDesktop = new DesktopConstraint(), LibId = new GuidRouteConstraint() }
            );

            routes.MapRoute("LibraryNode",
                "Library/{LibId}/{LibName}",
                new { controller = "Library", action = "Index", LibId = UrlParameter.Optional, LibName = UrlParameter.Optional },
                new { LibId = new GuidRouteConstraint() }); 
            #endregion

            //[Route("user/{userId:long:min(0)?}/{userName?}", Name = "User")]
            #region user
            routes.MapRoute("UserDesktop",
                "user/{userId}/{userName}",
                new { controller = "Home", action = "Index" },
                new { isDesktop = new DesktopConstraint() , userId = new LongRouteConstraint() }
            );

            routes.MapRoute("User",
                "user/{userId}/{userName}",
                new { controller = "Library", action = "Index", LibId = UrlParameter.Optional, LibName = UrlParameter.Optional },
                new { userId = new LongRouteConstraint()});
            #endregion


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