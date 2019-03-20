using Cloudents.Web.Models;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Cloudents.Web.Filters
{
    public class FormContentTypeAttribute : Attribute, IActionConstraint
    {
        public bool Accept(ActionConstraintContext context)
        {
            return context.RouteContext.HttpContext.Request.HasFormContentType;
        }

        public int Order => 0;
    }

    public class StartUploadingAttribute : Attribute, IActionConstraint
    {
        public bool Accept(ActionConstraintContext context)
        {
            //var sr = context.RouteContext.HttpContext.Request.Body;
            if (context.RouteContext.HttpContext.Request.HasFormContentType)
            {
                return false;
            }
            StreamReader reader = new StreamReader(context.RouteContext.HttpContext.Request.Body);
            JsonTextReader jsonReader = new JsonTextReader(reader);
            
            JsonSerializer ser = new JsonSerializer();
            var t = ser.Deserialize<UploadRequest>(jsonReader);
            context.RouteContext.HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);
            return t.Phase == UploadPhase.Start;

        }

        public int Order => 1;
    }
}
