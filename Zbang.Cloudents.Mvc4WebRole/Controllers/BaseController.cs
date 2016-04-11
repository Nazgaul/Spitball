﻿using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using StackExchange.Profiling;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ReadServices;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    public class BaseController : Controller
    {
        public IZboxWriteService ZboxWriteService { get; set; }
        public IZboxCacheReadService ZboxReadService { get; set; }

        protected CancellationTokenSource CreateCancellationToken(CancellationToken cancellationToken)
        {
            CancellationToken disconnectedToken = Response.ClientDisconnectedToken;
            return CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, disconnectedToken);
        }

        protected long? GetBoxIdRouteDataFromDifferentUrl(string url)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                {
                    return null;
                }
                var routeFromUrl =
                    RouteTable.Routes.GetRouteData(
                        new HttpContextWrapper(
                            new HttpContext(new HttpRequest(null, new UriBuilder(url).ToString(), string.Empty),
                                new HttpResponse(new StringWriter()))));
                if (routeFromUrl?.Values["boxId"] == null)
                {
                    return null;
                }
                long retVal;
                if (long.TryParse(routeFromUrl.Values["boxId"].ToString(), out retVal))
                {
                    return retVal;
                }
                return null;

            }
            catch (Exception ex)
            {
                TraceLog.WriteError("GetBoxIdRouteDataFromDifferentUrl url: " + url, ex);
                return null;
            }

        }

        protected string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);

                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString().Trim();
            }
        }

        protected bool IsCrawler()
        {
            return Regex.IsMatch(HttpContext.Request.UserAgent, @"bot|crawler|baiduspider|80legs|ia_archiver|voyager|curl|wget|yahoo! slurp|mediapartners-google", RegexOptions.IgnoreCase);
        }
        protected override void OnException(ExceptionContext filterContext)
        {
            var parameters = filterContext.HttpContext.Request.Params.ToString().Replace("&", "\n");
            var info =
                $"on exception base controller url {filterContext.HttpContext.Request.RawUrl} user {User.Identity.Name} ";
            TraceLog.WriteError(info, filterContext.Exception, parameters);
            base.OnException(filterContext);
        }
        protected override void HandleUnknownAction(string actionName)
        {
            var parameters = Request.Params.ToString().Replace("&", "\n");
            var info = string.Format("HandleUnknownAction {0} url {1} user {2} headers {5} isAjax {4} params {3} "
                , actionName, Request.RawUrl, User.Identity.Name, parameters, Request.IsAjaxRequest(), Request.Headers);


            TraceLog.WriteError(info);
            if (Request.IsAjaxRequest())
            {
                HttpNotFound().ExecuteResult(ControllerContext);
            }
            // View("Error").ExecuteResult(ControllerContext);
            base.HandleUnknownAction(actionName);
        }

        protected string GetErrorFromModelState()
        {
            return ModelState.SelectMany(x => x.Value.Errors.Select(error => error.ErrorMessage)).FirstOrDefault();
        }

        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding)
        {
            return Json(data, contentType, contentEncoding, JsonRequestBehavior.AllowGet);
            //return new JilJsonResult
            //{
            //    Data = data,
            //    ContentType = contentType,
            //    ContentEncoding = contentEncoding
            //};
            //return new JsonNetResult
            //{
            //    Data = data,
            //    ContentType = contentType,
            //    ContentEncoding = contentEncoding
            //};
        }
        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            var profiler = MiniProfiler.Current; // it's ok if this is null
            using (profiler.Step("Json render"))
            {
                return new JilJsonResult
                {
                    Data = data,
                    ContentType = contentType,
                    ContentEncoding = contentEncoding,
                    JsonRequestBehavior = behavior
                };
            }
        
        }

        protected JsonResult JsonOk(object data = null)
        {
            return Json(new JsonResponse(true, data));
        }
        protected JsonResult JsonError(object data = null)
        {
            return Json(new JsonResponse(false, data));
        }

        



        #region Language
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            TempDataProvider = new CookieTempDataProvider(HttpContext);
            //try
            //{
            //    if (!ControllerContext.IsChildAction)
            //    {
            //        UserLanguage.ChangeLanguage(requestContext.HttpContext, Server, requestContext.RouteData);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    TraceLog.WriteError("initialize", ex);
            //}
        }
        //protected void ChangeThreadLanguage(string language)
        //{
        //    if (!Languages.CheckIfLanguageIsSupported(language))
        //    {
        //        return;
        //    }
        //    if (Thread.CurrentThread.CurrentUICulture.Name == language) return;
        //    try
        //    {
        //        var cultureInfo = new CultureInfo(language);
        //        Thread.CurrentThread.CurrentUICulture = cultureInfo;
        //        Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureInfo.Name);
        //    }
        //    catch (CultureNotFoundException)
        //    {
        //    }
        //}

        #endregion
    }
}
