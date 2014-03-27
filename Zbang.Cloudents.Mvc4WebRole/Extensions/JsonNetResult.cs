using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Zbang.Cloudents.Mvc4WebRole.Extensions
{
    public class JsonNetSerializer
    {
        private JsonSerializerSettings SerializerSettings { get; set; }
        public JsonNetSerializer()
        {

            SerializerSettings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };
            SerializerSettings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
            SerializerSettings.Converters.Add(new IsoDateTimeConverter { DateTimeStyles = System.Globalization.DateTimeStyles.AssumeUniversal });

        }

        public void Serialize(System.IO.TextWriter output, object data)
        {
            JsonTextWriter writer = new JsonTextWriter(output)
            {
                Formatting = Newtonsoft.Json.Formatting.None
            };

            JsonSerializer serializer = JsonSerializer.Create(SerializerSettings);
            serializer.Serialize(writer, data);
        }

        public string Serialize(object data)
        {
            using (System.IO.TextWriter z = new System.IO.StringWriter())
            {
                Serialize(z, data);
                return z.ToString();
            }
        }


    }
    public class JsonNetResult : ActionResult
    {
        public Encoding ContentEncoding { get; set; }
        public string ContentType { get; set; }
        public object Data { get; set; }

        public JsonSerializerSettings SerializerSettings { get; set; }
        public Formatting Formatting { get; set; }

        public JsonNetResult()
        {
            SerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };
            SerializerSettings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
            SerializerSettings.Converters.Add(new IsoDateTimeConverter { DateTimeStyles = System.Globalization.DateTimeStyles.AssumeUniversal });
            Formatting = Newtonsoft.Json.Formatting.None;

        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            HttpResponseBase response = context.HttpContext.Response;

            response.ContentType = !string.IsNullOrEmpty(ContentType)
              ? ContentType
              : "application/json";

            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;

            if (Data != null)
            {
                JsonTextWriter writer = new JsonTextWriter(response.Output) { Formatting = Formatting };

                JsonSerializer serializer = JsonSerializer.Create(SerializerSettings);
                serializer.Serialize(writer, Data);

                writer.Flush();
            }
        }
    }
}