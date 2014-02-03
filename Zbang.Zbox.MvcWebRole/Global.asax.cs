using System;
using System.Web.Mvc;
using System.Web.Routing;
using Zbang.Zbox.Infrastructure.Routes;
using Zbang.Zbox.Infrastructure.Web.Ioc;
using System.Web;
using Zbang.Zbox.Infrastructure.ShortUrl;

namespace Zbang.Zbox.MvcWebRole
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            //on RoutesCollectionZbox class there are the same map route if you change in here you need to change in there as well
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.Add(new SubdomainRoute());
            routes.MapRoute(
                "DownloadAll",
                "DownloadBox/{boxid}",
                new { controller = "Storage", action = "DownloadAllBoxItem", boxId = "boxid" }
            );

            routes.MapRoute(
                "FileHandler",
                "DownloadBoxItem/{fileId}/{download}",
                new { controller = "AStorage", action = "DownloadBoxItem", download = UrlParameter.Optional }
            );

            routes.MapRoute(
                "ShareBox",
                "SharedBox/{boxid}",
                new { controller = "Collaboration", action = "SharedBox", boxId = "boxid" }
            );

            routes.MapRoute(
                "Index",
                "{boxid}",
                new { controller = "Home", action = "Index", boxId = "boxid" }
            );

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );


        }



        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RegisterRoutes(RouteTable.Routes);

            //RouteDebug.RouteDebugger.RewriteRoutesForTesting(RouteTable.Routes);
            //Replace default controller factory with Unity based custom factory
            var unityFactory = new UnityControllerFactory();
            ControllerBuilder.Current.SetControllerFactory(unityFactory);


            var cacheObj = unityFactory.Resolve<Zbang.Zbox.Infrastructure.Cache.ICache>();
            ShortCodesCache.Init(cacheObj);

        }

        protected void Application_EndRequest()
        {
            //Check for an Ajax call ending with a 302 status code
            if (Context.Response.StatusCode == 302 && Context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                //Clear response and replace status code with 401
                Context.Response.Clear();
                Context.Response.StatusCode = 401;
            }
        }

        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            //Check for an unaothorized Ajax call
            if (Response.StatusCode == 401 && Context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                //Add LoginUrl header so client code can redirect to login page
                Response.Clear();
                Response.AddHeader("LoginUrl", System.Web.Security.FormsAuthentication.LoginUrl);
            }
        }
    }


    public class SubdomainRoute : RouteBase
    {
        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            RouteData returnValue = null;

            // Retrieve the url - and split by dots:
            var url = httpContext.Request.Headers["HOST"];
            var index = url.IndexOf(".");

            // Determine if a subdomain is provided:
            if (index < 0)
                return returnValue;

            // Get the subdomain (as a string):
            string subDomain = url.Substring(0, index);

            // switch through each possible subdomain:
            switch (subDomain.ToLowerInvariant())
            {
                case "uploader":
                    returnValue = new RouteData(this, new MvcRouteHandler());
                    returnValue.Values.Add("controller", "AStorage");
                    returnValue.Values.Add("action", "UploadFile");

                    //returnValue.Values.Add("liveMode", false); // set parameter to 'false';
                    break;
                //case "live":
                //    returnValue = new RouteData(this, new MvcRouteHandler());
                //    returnValue.Values.Add("controller", "Database");
                //    returnValue.Values.Add("action", "Index");
                //    returnValue.Values.Add("liveMode", true); // set parameter to 'true';
                //    break;
                default: // not a supported domain, return null;
                    break;
            }

            return returnValue;
        }

        /// <summary>
        /// Required override. Just return null ;)
        /// </summary>
        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            return null;
        }
    }

}