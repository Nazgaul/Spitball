using System.IO;
using System.Linq;
using System.Web.Mvc;
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
        protected override void OnException(ExceptionContext filterContext)
        {
            var parameters = filterContext.HttpContext.Request.Params.ToString().Replace("&", "\n");
            var info = string.Format("on exception base controller url {0} user {1} ",
                filterContext.HttpContext.Request.RawUrl, User.Identity.Name);
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

        //protected IEnumerable<KeyValuePair<string, string[]>> GetModelStateErrors()
        //{
        //    return ModelState.ToDictionary(kvp => kvp.Key,
        //         kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()).Where(m => m.Value.Any());
        //}

        //protected IEnumerable<string> GetErrorsFromModelState()
        //{
        //    return ModelState.SelectMany(x => x.Value.Errors.Select(error => error.ErrorMessage));
        //}

        protected string GetErrorFromModelState()
        {
            return ModelState.SelectMany(x => x.Value.Errors.Select(error => error.ErrorMessage)).FirstOrDefault();
        }

        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding)
        {
            return new JsonNetResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding
            };
        }
        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonNetResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
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
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
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
