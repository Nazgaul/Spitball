using System;
using System.Text;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Cloudents.Mvc4WebRole.Filters
{
    [AttributeUsageAttribute(AttributeTargets.All, AllowMultiple = true)]
    internal class ZboxHandleErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
            {
                return;
            }
            var sb = new StringBuilder();
            foreach (var formKey in filterContext.HttpContext.Request.Form.AllKeys)
            {
                sb.AppendLine($"key: {formKey} value: {filterContext.HttpContext.Request.Form[formKey]}");
            }
            var sb2 = new StringBuilder();
            foreach (var formKey in filterContext.HttpContext.Request.QueryString.AllKeys)
            {
                sb2.AppendLine($"key: {formKey} value: {filterContext.HttpContext.Request.Form[formKey]}");
            }

            var info =
                $"url {filterContext.HttpContext.Request.RawUrl} user {filterContext.HttpContext.User.Identity.Name}  RequestType {filterContext.HttpContext.Request.RequestType} formData {sb} querystring {sb2}";
            TraceLog.WriteError(info, filterContext.Exception);

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
