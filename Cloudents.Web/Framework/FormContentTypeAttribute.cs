using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text;

namespace Cloudents.Web.Framework
{
    public class FormContentTypeAttribute : Attribute, IActionConstraint
    {
        public bool Accept(ActionConstraintContext context)
        {
            return context.RouteContext.HttpContext.Request.HasFormContentType;
        }

        public int Order => 0;
    }

    //public class StartUploadingAttribute : Attribute, IActionConstraint
    //{
    //    public bool Accept(ActionConstraintContext context)
    //    {
    //        //var sr = context.RouteContext.HttpContext.Request.Body;
    //        if (context.RouteContext.HttpContext.Request.HasFormContentType)
    //        {
    //            return false;
    //        }

    //        var reader = new HttpRequestStreamReader(context.RouteContext.HttpContext.Request.Body, Encoding.UTF8);
    //        var parser = JObject.Parse(reader.ReadToEnd());

    //        var t = string.Equals(parser.GetValue("phase", StringComparison.OrdinalIgnoreCase).Value<string>(), "start", StringComparison.OrdinalIgnoreCase);
    //        context.RouteContext.HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);
    //        return t;
    //    }

    //    public int Order => 1;
    //}
}
