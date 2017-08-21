using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Zbang.Cloudents.MobileApp.Filters
{
    public class JsonSerializeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);
            var jsonFormatter = actionExecutedContext.ActionContext.ControllerContext.Configuration.Formatters.JsonFormatter;

            jsonFormatter.SerializerSettings.ContractResolver =
                new CamelCasePropertyNamesContractResolver();
            jsonFormatter.SerializerSettings.NullValueHandling =
                Newtonsoft.Json.NullValueHandling.Ignore;
            var enumConvertor = jsonFormatter.SerializerSettings.Converters.OfType<StringEnumConverter>().FirstOrDefault();
            if (enumConvertor == null)
            {
                jsonFormatter.SerializerSettings.Converters.Add(new StringEnumConverter { CamelCaseText = false });
            }
            else
            {
                enumConvertor.CamelCaseText = true;
            }
           
            //jsonFormatter.SerializerSettings.Converters.Add(new StringEnumConverter { CamelCaseText = false });

            var formatter = jsonFormatter.SerializerSettings.Converters.OfType<IsoDateTimeConverter>().FirstOrDefault();
            if (formatter == null)
            {
                var iso = new IsoDateTimeConverter
                {
                    DateTimeStyles = System.Globalization.DateTimeStyles.AssumeUniversal,
                    DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'"
                };
                //var isoSettings = jsonFormatter.SerializerSettings.Converters.OfType<IsoDateTimeConverter>().Single();
                jsonFormatter.SerializerSettings.Converters.Add(iso);
            }
            else
            {
                formatter.DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'";
            }
        }
    }
}