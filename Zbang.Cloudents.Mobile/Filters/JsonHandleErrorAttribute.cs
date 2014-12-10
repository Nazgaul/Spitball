using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Zbang.Cloudents.Mobile.Helpers;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Cloudents.Mvc4WebRole.Filters
{
    public class JsonHandleErrorAttribute : HandleErrorAttribute
    {
        public HttpStatusCode HttpStatus { get; set; }

        public string HttpDescription { get; set; }

        public override void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsAjaxRequest())
            {
                return;
            }
            if (!ExceptionType.IsInstanceOfType(filterContext.Exception))
            {
                return;
            }
            var sb = new StringBuilder();
            foreach (var formKey in filterContext.HttpContext.Request.Form.AllKeys)
            {
                sb.AppendLine(string.Format("key: {0} value: {1}", formKey,
                    filterContext.HttpContext.Request.Form[formKey]));
            }

            var info = string.Format("url {0} user {1}  RequestType {2} formData {3}", filterContext.HttpContext.Request.RawUrl,
                filterContext.HttpContext.User.Identity.Name,
                filterContext.HttpContext.Request.RequestType,
                sb
                );
            TraceLog.WriteError(info, filterContext.Exception, filterContext.HttpContext.Request.Params.ToString().Replace("&", "\n"));
            var headerSend = filterContext.HttpContext.Items[HTTPItemConsts.HeaderSend];
            if (headerSend != null && (bool)headerSend)
            {
                base.OnException(filterContext);
                return;
            }
            filterContext.ExceptionHandled = true;

            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = (int)HttpStatus;
            filterContext.HttpContext.Response.StatusDescription = HttpDescription;

            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            

        }
    }
}