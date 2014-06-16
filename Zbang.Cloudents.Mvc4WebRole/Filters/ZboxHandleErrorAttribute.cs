using System;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Cloudents.Mvc4WebRole.Filters
{
    internal class ZboxHandleErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {

            var info = string.Format("url {0} user {1}  RequestType {2}", filterContext.HttpContext.Request.RawUrl,
                filterContext.HttpContext.User.Identity.Name,
                filterContext.HttpContext.Request.RequestType
                );
            TraceLog.WriteError(info, filterContext.Exception, filterContext.HttpContext.Request.Params.ToString().Replace("&", "\n"));

            //if (filterContext.Exception.GetType() == typeof(BoxAccessDeniedException))
            //{

            //}
            //Exception exception = filterContext.Exception;
            //if (exception == null)
            //    return;
            //if (exception is TargetInvocationException)
            //    exception = exception.InnerException;

            //// If this is not a ResourceNotFoundException error, ignore it.
            //if (!(exception is ResourceNotFoundException))
            //    return;

            //filterContext.Result = new ViewResult()
            //{
            //    TempData = controller.TempData,
            //    ViewName = View
            //};

            //filterContext.ExceptionHandled = true;
            //filterContext.HttpContext.Response.Clear();
            //filterContext.HttpContext.Response.StatusCode = 404;



            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.ExceptionHandled = true;
                var headerSend = filterContext.HttpContext.Items[HTTPItemConsts.HeaderSend];
                if (headerSend != null && (bool)headerSend)
                {
                    base.OnException(filterContext);
                    return;
                }
                try
                {
                    filterContext.ExceptionHandled = true;

                    filterContext.HttpContext.Response.Clear();
                    filterContext.HttpContext.Response.StatusCode = 500;
                    filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError("Zbox Handle Error Attribute trying to send header", ex);

                }
            }
            base.OnException(filterContext);
        }
    }
}
