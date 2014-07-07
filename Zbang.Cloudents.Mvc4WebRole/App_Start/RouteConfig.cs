//using Microsoft.AspNet.SignalR;

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
                  new { controller = "Box", action = "IndexDesktop" },
                  new { isDesktop = new DesktopConstraint(), boxId = new LongRouteConstraint() }
              );
            routes.MapRoute("CourseBoxDesktop",
              "course/{universityName}/{boxId}/{boxName}",
              new { controller = "Box", action = "IndexDesktop" },
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
              "Bootstrap",
              "bootstrap.js",
              new { controller = "Home", action = "Bootstrap", index = UrlParameter.Optional }
              );

            routes.MapRoute(
              "Sitemap1",
              "sitemap-{index}.xml",
              new { controller = "Home", action = "SiteMap", index = UrlParameter.Optional }
              );

            #region library
            routes.MapRoute("LibraryDesktop",
                "library/{LibId}/{LibName}",
                new { controller = "Home", action = "Index" , LibId = UrlParameter.Optional, LibName= UrlParameter.Optional },
                new { isDesktop = new DesktopConstraint(), LibId = new NullGuidConstrait() }
            );

            routes.MapRoute("LibraryNode",
                "Library/{LibId}/{LibName}",
                new { controller = "Library", action = "Index", LibId = UrlParameter.Optional, LibName = UrlParameter.Optional },
                new { LibId = new NullGuidConstrait() }); 
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
                new { controller = "User", action = "Index", userId = UrlParameter.Optional, userName = UrlParameter.Optional },
                new { userId = new LongRouteConstraint()});
            #endregion

            //[Route("Item/{universityName}/{boxId:long}/{boxName}/{itemid:long:min(0)}/{itemName}", Name = "Item")]

            #region item
            routes.MapRoute("ItemDesktop",
                "item/{universityName}/{boxId}/{boxName}/{itemid}/{itemName}",
                new { controller = "Item", action = "IndexDesktop" },
                new { isDesktop = new DesktopConstraint(), boxId = new LongRouteConstraint(), itemid = new LongRouteConstraint() }
            );

            routes.MapRoute("Item",
                "item/{universityName}/{boxId}/{boxName}/{itemid}/{itemName}",
                new { controller = "Item", action = "Index" },
                new
                {
                    boxId = new LongRouteConstraint(),
                    itemid = new LongRouteConstraint()
                }
                );
            #endregion

            #region quiz
            //[Route("Quiz/{universityName}/{boxId:long}/{boxName}/{quizId:long:min(0)}/{quizName}", Name = "Quiz")]
            routes.MapRoute("QuizDesktop",
                "Quiz/{universityName}/{boxId}/{boxName}/{quizId}/{quizName}",
                new { controller = "Quiz", action = "IndexDesktop" },
                new { isDesktop = new DesktopConstraint(), boxId = new LongRouteConstraint(), quizId = new LongRouteConstraint() }
            );

            routes.MapRoute("Quiz",
                "Quiz/{universityName}/{boxId}/{boxName}/{quizId}/{quizName}",
                new
                {
                    controller = "Quiz",
                    action = "Index"
                },
                new
                {
                    boxId = new LongRouteConstraint(),
                    quizId = new LongRouteConstraint()
                }
                );
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