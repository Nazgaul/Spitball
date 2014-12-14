//using Microsoft.AspNet.SignalR;

using System.Web.Mvc;
using System.Web.Mvc.Routing;
using System.Web.Mvc.Routing.Constraints;
using System.Web.Routing;
using Zbang.Cloudents.Mvc4WebRole.Helpers;

namespace Zbang.Cloudents.Mvc4WebRole
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.LowercaseUrls = true;
            routes.AppendTrailingSlash = true;

            var constraintsResolver = new DefaultInlineConstraintResolver();
            routes.MapMvcAttributeRoutes(constraintsResolver);


            routes.MapRoute("AccountLanguage",
                "account/{lang}",
                new { controller = "Account", action = "Index", lang = UrlParameter.Optional },
                new { lang = "^[A-Za-z]{2}-[A-Za-z]{2}$" });




            //routes.MapRoute("AccountSettingsDesktop",
            //    "account/settings",
            //    new {controller = "Account", action = "SettingsDesktop"},
            //    new { isDesktop = new DesktopConstraint() });

            #region Box

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
            routes.MapRoute("shortBox",
                "box/{box62Id}",
                new {controller = "Box", action = "ShortUrl"});

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




            //[Route("Item/{universityName}/{boxId:long}/{boxName}/{itemid:long:min(0)}/{itemName}", Name = "Item")]

            #region item

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
            //routes.MapRoute("QuizDesktop",
            //    "Quiz/{universityName}/{boxId}/{boxName}/{quizId}/{quizName}",
            //    new { controller = "Quiz", action = "IndexDesktop" },
            //    new { isDesktop = new DesktopConstraint(), boxId = new LongRouteConstraint(), quizId = new LongRouteConstraint() }
            //);

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

            routes.MapRoute("LibraryDesktop",
                "library",
                new { controller = "Home", action = "Index" }
            );
            routes.MapRoute("LibraryDesktop2",
                "library/{LibId}/{LibName}",
                new { controller = "Home", action = "Index" }
            );
            routes.MapRoute("User",
               "user/{userId}/{userName}",
               new { controller = "Home", action = "Index" },
               new { userId = new LongRouteConstraint() }
           );
            routes.MapRoute("Search",
              "search",
              new { controller = "Home", action = "Index" }
          );

            routes.MapRoute("AccountSettings",
             "account/settings",
             new { controller = "Home", action = "Index" }
         );


            routes.MapRoute("Default", "{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );


        }
    }
}