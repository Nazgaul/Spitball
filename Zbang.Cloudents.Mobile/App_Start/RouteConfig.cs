//using Microsoft.AspNet.SignalR;

using System.Web.Mvc;
using System.Web.Mvc.Routing;
using System.Web.Mvc.Routing.Constraints;
using System.Web.Routing;

namespace Zbang.Cloudents.Mobile
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
           
            // routes.MapRoute("LibraryMobile",
            //    "department",
            //    new { controller = "Library", action = "DepartmentRedirect" },
            //    new { isDesktop = new MobileConstraint() }
            //);
           
            // routes.MapRoute("LibraryAjax",
            //    "library",
            //    new { controller = "Library", action = "Index" },
            //    new { isDesktop = new AjaxConstaint() }
            //);

            #endregion

            //[Route("user/{userId:long:min(0)?}/{userName?}", Name = "User")]
            #region user
           

            routes.MapRoute("User",
                "user/{userId}/{userName}",
                new { controller = "User", action = "Index", userId = UrlParameter.Optional, userName = UrlParameter.Optional },
                new { userId = new LongRouteConstraint() });
            #endregion

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

            routes.MapRoute("Default", "{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );


        }
    }
}