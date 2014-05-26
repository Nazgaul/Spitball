using System;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Cloudents.Mvc4WebRole.Filters
{
    public class FlushHeaderAttribute : ActionFilterAttribute
    {
        public string PartialViewName { get; set; }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Result.GetType() == typeof(RedirectToRouteResult))
            {
                base.OnActionExecuted(filterContext);
                return;
            }
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                base.OnActionExecuted(filterContext);
                return;
            }
            
            var headerSend = filterContext.HttpContext.Items[HTTPItemConsts.HeaderSend];
            if (headerSend != null && (bool)headerSend)
            {
                base.OnActionExecuted(filterContext);
                return;
            }

            try
            {
                var partialViewName = PartialViewName ?? "_Header";
                new PartialViewResult { ViewName = partialViewName }.ExecuteResult(filterContext);
                System.Web.Helpers.AntiForgery.GetHtml();
                filterContext.HttpContext.Response.Flush();

                filterContext.Controller.ViewBag.Flush = true;
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("Flush Header Attribute trying to send header", ex);
            }
            base.OnActionExecuted(filterContext);
        }

    }
}