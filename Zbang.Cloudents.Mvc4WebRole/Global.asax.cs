using System;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.WindowsAzure.ServiceRuntime;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Cloudents.Mvc4WebRole.Controllers;
using Zbang.Cloudents.Mvc4WebRole.Models.Account;
using Zbang.Zbox.Infrastructure.Extensions;

//using Microsoft.AspNet.SignalR.ServiceBus;

namespace Zbang.Cloudents.Mvc4WebRole
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
#if DEBUG
            log4net.Config.XmlConfigurator.Configure();
#endif
            DisplayConfig.RegisterDisplays();
            ViewConfig.RegisterEngineAndViews();
            BundleRegistration.RegisterBundles();
            //BundleConfig.RegisterBundles();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            RouteConfig.RegisterRoutes(RouteTable.Routes);

            System.Web.Helpers.AntiForgeryConfig.RequireSsl = true;
#if DEBUG
            System.Web.Helpers.AntiForgeryConfig.RequireSsl = false;
#endif
            System.Web.Helpers.AntiForgeryConfig.CookieName = "sbVrfctn";
            System.Web.Helpers.AntiForgeryConfig.SuppressXFrameOptionsHeader = true;

            //this disable identity check
            System.Web.Helpers.AntiForgeryConfig.SuppressIdentityHeuristicChecks = true;
            MvcHandler.DisableMvcResponseHeader = true;
            TelemetryConfiguration.Active.InstrumentationKey = ConfigFetcher.Fetch("APPINSIGHTS_INSTRUMENTATIONKEY");
        }


        protected void Application_End()
        {
            TraceLog.WriteInfo("Application ending");
        }

        protected void Application_BeginRequest()
        {
        }

        protected void Application_EndRequest()
        {
        }

        public override string GetVaryByCustomString(HttpContext context, string custom)
        {
            try
            {
                var keys = custom.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                Array.Sort(keys);
                var value = string.Empty;
                foreach (var key in keys)
                {
                    if (key == CustomCacheKeys.Auth)
                    {
                        if (User?.Identity != null)
                        {
                            value += User.Identity.IsAuthenticated.ToString(CultureInfo.InvariantCulture);
                        }
                    }
                    if (key == CustomCacheKeys.Lang)
                    {
                        value += Thread.CurrentThread.CurrentUICulture.Name;
                    }
                    if (key == CustomCacheKeys.University)
                    {
                        value += context.Request.Cookies[UniversityCookie.CookieName]?.Value;
                    }
                    if (key == CustomCacheKeys.Promo)
                    {
                        value += context.Request.Cookies[UniversityFlashcardPromo.CookieName]?.Value;
                    }
                    //if (key == CustomCacheKeys.Url)
                    //{
                    //    value += context.Request.Url.AbsolutePath;
                    //}
                }
                return string.IsNullOrWhiteSpace(value) ? base.GetVaryByCustomString(context, custom) : value;
            }
            catch (Exception ex)
            {
                TraceLog.WriteError($"GetVaryByCustomString custom {custom} context {context}", ex);
                throw;
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var err = HttpContext.Current.Server.GetLastError();
            var parameters = HttpContext.Current.Request.Params.ToString().Replace("&", "\n");
            var url = HttpContext.Current.Request.Url.AbsolutePath;
            TraceLog.WriteError("Application error - url: " + url + "\n - params: " + parameters, err);

            var httpContext = ((MvcApplication)sender).Context;
            var currentController = " ";
            var currentAction = " ";
            var currentRouteData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));

            if (currentRouteData != null)
            {
                if (!string.IsNullOrEmpty(currentRouteData.Values["controller"]?.ToString()))
                {
                    currentController = currentRouteData.Values["controller"].ToString();
                }

                if (!string.IsNullOrEmpty(currentRouteData.Values["action"]?.ToString()))
                {
                    currentAction = currentRouteData.Values["action"].ToString();
                }
            }

            var ex = Server.GetLastError();
            var controller = new ErrorController();
            var routeData = new RouteData();
            var action = "Index";

            if (ex is HttpException)
            {
                var httpEx = ex as HttpException;

                switch (httpEx.GetHttpCode())
                {
                    case 404:
                        action = "Error";
                        break;

                    // others if any
                }
            }

            httpContext.ClearError();
            httpContext.Response.Clear();
            if (HttpContext.Current.Items[HTTPItemConsts.HeaderSend] == null)
            {
                httpContext.Response.StatusCode = ex is HttpException ? ((HttpException)ex).GetHttpCode() : 500;
            }
            httpContext.Response.TrySkipIisCustomErrors = true;

            routeData.Values["controller"] = "Error";
            routeData.Values["action"] = action;
            if (Request.HttpMethod.ToUpper() != "GET")
            {
                return;
            }
            //var uriBuilder = new UriBuilder(HttpContext.Current.Request.Url) {Path = "/error"};

            controller.ViewData.Model = new HandleErrorInfo(ex, currentController, currentAction);
            try
            {
             //   HttpContext.Current.Response.Redirect(uriBuilder.ToString());
                //Response.Redirect("/error");
                //((IController)controller).Execute(new RequestContext(new HttpContextWrapper(httpContext), routeData));//(new HttpContextWrapper(httpContext), routeData));
            }
            catch (Exception ex2)
            {
                TraceLog.WriteError("on application error", ex2);
            }
        }

        //protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        //{
        //    HttpContext.Current.Response.Headers.Remove("Server");
        //}
        protected void Application_PreSendRequestHeaders(object source, EventArgs e)
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Items[HTTPItemConsts.HeaderSend] = true;
            }
        }
    }
}