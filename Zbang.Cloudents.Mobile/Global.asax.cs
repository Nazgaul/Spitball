﻿using System;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.WebPages;
using Zbang.Cloudents.Mobile.Controllers;
using Zbang.Cloudents.Mobile.Helpers;
using Zbang.Cloudents.SiteExtension;
using Zbang.Zbox.Infrastructure.Trace;

//using Microsoft.AspNet.SignalR.ServiceBus;

namespace Zbang.Cloudents.Mobile
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            UnityConfig.RegisterTypes();
#if DEBUG
            log4net.Config.XmlConfigurator.Configure();
#endif
            //don't need this
            //AreaRegistration.RegisterAllAreas();

            //IocConfig.RegisterIoc();
            //DisplayConfig.RegisterDisplays();
            DisplayModeProvider.Instance.Modes.Clear();
            DisplayModeProvider.Instance.Modes.Add(new DefaultDisplayMode(string.Empty));

            ViewConfig.RegisterEngineAndViews();
            BundleRegistration.RegisterBundles();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);


            RouteConfig.RegisterRoutes(RouteTable.Routes);
            
            System.Web.Helpers.AntiForgeryConfig.RequireSsl = true;
            System.Web.Helpers.AntiForgeryConfig.CookieName = "cdVrfctn";
            System.Web.Helpers.AntiForgeryConfig.SuppressXFrameOptionsHeader = true;

            //this disable identity check
            System.Web.Helpers.AntiForgeryConfig.SuppressIdentityHeuristicChecks = true;
            MvcHandler.DisableMvcResponseHeader = true;
        }
        protected void Application_End()
        {
            TraceLog.WriteInfo("Application ending");
        }




        //for output cache
        //protected void Application_AcquireRequestState()
        //{
        //    try
        //    {
        //        UserLanguage.ChangeLanguage(new HttpContextWrapper(HttpContext.Current),
        //            new HttpServerUtilityWrapper(HttpContext.Current.Server), null);
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceLog.WriteError("initialize", ex);
        //    }
        //}

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
                        value += User.Identity.IsAuthenticated.ToString(CultureInfo.InvariantCulture);
                    }
                    if (key == CustomCacheKeys.Lang)
                    {
                        value += Thread.CurrentThread.CurrentUICulture.Name;
                    }
                    if (key == CustomCacheKeys.IsAjax)
                    {
                        value += context.Request.Headers["X-Requested-With"];
                    }
                }
                if (string.IsNullOrWhiteSpace(value))
                {
                    return base.GetVaryByCustomString(context, custom);
                }
                return value;
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("GetVaryByCustomString custom {0} context {1}", custom, context), ex);
                throw;
            }
        }


        protected void Application_Error(object sender, EventArgs e)
        {
            var err = HttpContext.Current.Server.GetLastError();
            var parameters = HttpContext.Current.Request.Params.ToString().Replace("&", "\n");
            var url = HttpContext.Current.Request.Url.AbsolutePath;
            TraceLog.WriteError("Application error - url: " + url + " - params: " + parameters, err);


            var httpContext = ((MvcApplication)sender).Context;
            var currentController = " ";
            var currentAction = " ";
            var currentRouteData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));

            if (currentRouteData != null)
            {
                if (currentRouteData.Values["controller"] != null && !String.IsNullOrEmpty(currentRouteData.Values["controller"].ToString()))
                {
                    currentController = currentRouteData.Values["controller"].ToString();
                }

                if (currentRouteData.Values["action"] != null && !String.IsNullOrEmpty(currentRouteData.Values["action"].ToString()))
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
            controller.ViewData.Model = new HandleErrorInfo(ex, currentController, currentAction);
            ((IController)controller).Execute(new RequestContext(new HttpContextWrapper(httpContext), routeData));
        }

        //protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        //{
        //    HttpContext.Current.Response.Headers.Remove("Server");
        //}
        protected void Application_PreSendRequestHeaders(Object source, EventArgs e)
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Items[HTTPItemConsts.HeaderSend] = true;
            }
        }


    }
}