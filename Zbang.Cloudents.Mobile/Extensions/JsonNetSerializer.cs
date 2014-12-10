using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Zbang.Cloudents.Mvc4WebRole.Extensions
{
    public class JsonNetSerializer
    {
        private JsonSerializerSettings SerializerSettings { get; set; }
        public JsonNetSerializer()
        {

            SerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };
            SerializerSettings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
            SerializerSettings.Converters.Add(new IsoDateTimeConverter { DateTimeStyles = System.Globalization.DateTimeStyles.AssumeUniversal });

        }

        private void Serialize(System.IO.TextWriter output, object data)
        {
            using (var writer = new JsonTextWriter(output)
            {
                Formatting = Formatting.None
            })
            {
                var serializer = JsonSerializer.Create(SerializerSettings);
                serializer.Serialize(writer, data);
            }
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
}