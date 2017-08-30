using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Web.Mvc;

namespace Zbang.Cloudents.Mvc4WebRole.Extensions
{
    public class JsonNetResult : JsonResult
    {
        public static JsonSerializerSettings SerializerSettings { get; set; }

        static JsonNetResult()
        {
            SerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };
            SerializerSettings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
            SerializerSettings.Converters.Add(new IsoDateTimeConverter { DateTimeStyles = System.Globalization.DateTimeStyles.AssumeUniversal });
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var response = context.HttpContext.Response;

            response.ContentType = !string.IsNullOrEmpty(ContentType)
              ? ContentType
              : "application/json";

            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;

            if (Data == null) return;
            using (var writer = new JsonTextWriter(response.Output) { Formatting = Formatting.None })
            {
                JsonSerializer serializer = JsonSerializer.Create(SerializerSettings);
                serializer.Serialize(writer, Data);
                writer.Flush();
            }
        }
    }
}