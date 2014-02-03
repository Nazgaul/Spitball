using System;
using System.IO;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Zbang.Cloudents.Mvc4WebRole.Extensions
{
    public class CdJsonResult : ActionResult
    {
       
        private JsonSerializerSettings Settings { get; set; }

        public object Data { get; set; }
        //public JsonRequestBehavior JsonRequestBehavior { get; set; }

        public CdJsonResult()
        {
            Settings = new JsonSerializerSettings
                {
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    Formatting = Formatting.None,
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Converters = new[] { new Newtonsoft.Json.Converters.StringEnumConverter() }

                };
            //JsonRequestBehavior = JsonRequestBehavior.DenyGet;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            //if (JsonRequestBehavior == JsonRequestBehavior.DenyGet &&
            //    String.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            //{
            //    throw new InvalidOperationException("Json get is not allowed");
            //}

            var response = context.HttpContext.Response;

            response.ContentType = "application/json";

            if (Data == null)
            {
                return;
            }

            var serailizer = JsonSerializer.Create(Settings);
            using (var sw = new StringWriter())
            {
                serailizer.Serialize(sw, Data);
                response.Write(sw.ToString());
            }
        }

    }



}