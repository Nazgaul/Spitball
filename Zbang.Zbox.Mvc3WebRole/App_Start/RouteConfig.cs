using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Zbang.Zbox.Mvc3WebRole.Routing;

namespace Zbang.Zbox.Mvc3WebRole.App_Start
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.Add(new SubdomainRoute());

            routes.MapRoute(
                "DownloadAll",
                "DownloadBox/{boxid}",
                new { controller = "Storage", action = "DownloadAllBoxItem", boxId = "boxid" }
            );

            routes.MapRoute(
               "FileHandler",
               "D/{BoxUid}/{ItemUid}",
               new { controller = "Item", action = "Download", download = UrlParameter.Optional });


            //routes.MapRoute(
            //    "LibraryNode",
            //    "Library/{section}/{title}",
            //    new { controller = "Library", action = "Index", title = UrlParameter.Optional }//,
            //    //new { friendUid = new FriendRoute() }
            //    );

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
            //routes.MapRoute(
            //   "DefaultLanguage",
            //   "{language}/{controller}/{action}/{id}",
            //   new { language = "en", controller = "Home", action = "Index", id = "" }
            //   );

        }


        //public class SubdomainRoute : RouteBase
        //{
        //    public override RouteData GetRouteData(HttpContextBase httpContext)
        //    {
        //        RouteData returnValue = null;

        //        // Retrieve the url - and split by dots:
        //        var url = httpContext.Request.Headers["HOST"];
        //        var index = url.IndexOf(".");

        //        // Determine if a subdomain is provided:
        //        if (index < 0)
        //            return returnValue;

        //        // Get the subdomain (as a string):
        //        string subDomain = url.Substring(0, index);

        //        // switch through each possible subdomain:
        //        switch (subDomain.ToLowerInvariant())
        //        {
        //            case "upload":
        //                returnValue = new RouteData(this, new MvcRouteHandler());
        //                returnValue.Values.Add("controller", "Upload");
        //                if (httpContext.Request.Path == "/UploadFile")
        //                {
        //                    returnValue.Values.Add("action", "UploadFile");
        //                    break;
        //                }

        //                returnValue.Values.Add("action", "Index");
        //                //returnValue.Values.Add("liveMode", false); // set parameter to 'false';
        //                break;
        //            //case "live":
        //            //    returnValue = new RouteData(this, new MvcRouteHandler());
        //            //    returnValue.Values.Add("controller", "Database");
        //            //    returnValue.Values.Add("action", "Index");
        //            //    returnValue.Values.Add("liveMode", true); // set parameter to 'true';
        //            //    break;
        //            default: // not a supported domain, return null;
        //                break;
        //        }

        //        return returnValue;
        //    }

        //    /// <summary>
        //    /// Required override. Just return null ;)
        //    /// </summary>
        //    public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        //    {
        //        return null;
        //    }
        //}
    }
}