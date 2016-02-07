//using Microsoft.AspNet.SignalR;

using System.Web.Mvc;
using System.Web.Mvc.Routing;
using System.Web.Mvc.Routing.Constraints;
using System.Web.Routing;
using Zbang.Zbox.Infrastructure.Consts;

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
                "{lang}",
                new { controller = "Home", action = "Index", lang = UrlParameter.Optional },
                new { lang = "^[A-Za-z]{2}-[A-Za-z]{2}$" });

            routes.MapRoute("signin",
              "account/signin",
              new { controller = "Home", action = "Index" }
              );
            routes.MapRoute("signup",
             "account/signup",
             new { controller = "Home", action = "Index" }
             );
            routes.MapRoute("resetpassword",
            "account/resetpassword",
            new { controller = "Home", action = "Index" }
            );
           


            //[Route("account/signin/")]
            //[Route("account/resetpassword/")]
            //[Route("account/signup")]

            #region Box

            routes.MapRoute("PrivateBox",
            "box/my/{boxId}/{boxName}",
            new { controller = "Box", action = "Index" },
            new { boxId = new LongRouteConstraint() }
            );
            routes.MapRoute("PrivateBoxWithSub",
           "box/my/{boxId}/{boxName}/{part}",
           new { controller = "Box", action = "Index" },
           new { boxId = new LongRouteConstraint(), part = "feed|items|quizzes|members" }
           );
           

            routes.MapRoute("CourseBox",
              "course/{universityName}/{boxId}/{boxName}",
              new { controller = "Box", action = "Index" },
              new { boxId = new LongRouteConstraint() }
            );
            routes.MapRoute("CourseBoxWithSub",
              "course/{universityName}/{boxId}/{boxName}/{part}",
              new { controller = "Box", action = "Index" },
              new { boxId = new LongRouteConstraint(), part = "feed|items|quizzes|members" }
            );
         

            routes.MapRoute("shortBox",
                UrlConsts.ShortBox,
                new { controller = "Box", action = "ShortUrl" });

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
            routes.MapRoute("shortItem",
               UrlConsts.ShortItem,
               new { controller = "Item", action = "ShortUrl" });
            #endregion

            #region quiz

            routes.MapRoute("QuizCreate",
                "box/my/{boxId}/{boxName}/quizcreate",
                new { controller = "Home", action = "IndexEmpty" });

            routes.MapRoute("QuizCreate2",
                "course/{universityName}/{boxId}/{boxName}/quizcreate",
                new { controller = "Home", action = "IndexEmpty" });

            routes.MapRoute("Quiz",
                "quiz/{universityName}/{boxId}/{boxName}/{quizId}/{quizName}",
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
                new { controller = "Home", action = "IndexEmpty" }
            );

            routes.MapRoute("LibraryDesktop2",
                "library/{LibId}/{LibName}",
                new { controller = "Home", action = "IndexEmpty" }
            );
            routes.MapRoute("User",
               "user/{userId}/{userName}",
               new { controller = "Home", action = "IndexEmpty" },
               new { userId = new LongRouteConstraint() }
            );
            //routes.MapRoute("Search",
            //  "search",
            //  new { controller = "Home", action = "IndexEmpty" }
            //);

            routes.MapRoute("AccountSettings",
             "account/settings",
             new { controller = "Home", action = "IndexEmpty" }
            );
            routes.MapRoute("AccountSettingsPart",
            "account/settings/{part}",
            new { controller = "Home", action = "IndexEmpty" },
            new { part = "info|password|notification" }
           );

            routes.MapRoute("Blog2",
             "blog",
             new { controller = "Home", action = "IndexEmpty" }
         );
            routes.MapRoute("Blog",
              "blog/{lang}",
              new { controller = "Home", action = "IndexEmpty", lang = UrlParameter.Optional },
              new { lang = "^[A-Za-z]{2}-[A-Za-z]{2}$" }
          );

            routes.MapRoute("Jobs",
              "jobs",
              new { controller = "Home", action = "IndexEmpty" }
            );

            routes.MapRoute("Help",
             "help",
             new { controller = "Home", action = "IndexEmpty" }
           );
            //routes.MapRoute("TOS",
            //"terms",
            //new { controller = "Home", action = "Terms" }
            //);
            

            routes.MapRoute("Default", "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );


        }
    }
}