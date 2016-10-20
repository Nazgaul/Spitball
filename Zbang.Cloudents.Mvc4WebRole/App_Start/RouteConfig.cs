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

            routes.MapRoute("homePage",
             "",
             new { controller = "Home", action = "Index" }
             );

            routes.MapRoute("universityLibrary",
                "university/{universityId}/{universityName}",
                new {controller = "University", action = "Index" },
                new { /*auth = new AuthorizationConstraint() ,*/ universityId = new LongRouteConstraint() }
             );

            routes.MapRoute("universityLibraryNodes",
                "university/{universityId}/{universityName}/{libraryname}",
                new { controller = "Home", action = "IndexEmpty" },
                new { universityId = new LongRouteConstraint() }
             );

            routes.MapRoute("ClassChoose",
               "course/choose/",
               new { controller = "Home", action = "IndexEmpty" }
            );

            routes.MapRoute("resetpassword",
           "account/resetpassword",
           new { controller = "Home", action = "Index" }, new { httpMethod = new HttpMethodConstraint("GET") }
           );

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

            #region quiz

            routes.MapRoute("QuizCreate",
                "box/my/{boxId}/{boxName}/quizcreate",
                new { controller = "Home", action = "IndexEmpty" });

            routes.MapRoute("QuizCreate2",
                "course/{universityName}/{boxId}/{boxName}/quizcreate",
                new { controller = "Home", action = "IndexEmpty" });

            
            #endregion

            routes.MapRoute("AccountSettings",
             "account/settings",
             new { controller = "Home", action = "IndexEmpty" }
            );
            routes.MapRoute("AccountSettingsPart",
            "account/settings/{part}",
            new { controller = "Home", action = "IndexEmpty" },
            new { part = "info|password|notification|department" }
           );
      
            routes.MapRoute("Blog2",
             "blog",
             new { controller = "Home", action = "Blog" }
            );
            routes.MapRoute("Blog",
              "blog/{lang}",
              new { controller = "Home", action = "IndexEmpty", lang = UrlParameter.Optional },
              new { lang = "^[A-Za-z]{2}-[A-Za-z]{2}$" }
            );

            routes.MapRoute("UniversityLink", "{universityName}",
                new {controller = "Home", action = "Index"},
                new { universityName = new UniversityConstraint() });

            routes.MapRoute("Default", "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );


        }
    }
}