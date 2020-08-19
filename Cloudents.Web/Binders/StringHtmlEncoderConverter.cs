using System;
using Newtonsoft.Json;

namespace Cloudents.Web.Binders
{
    public class StringHtmlEncoderConverter : JsonConverter<string>
    {
        public override void WriteJson(JsonWriter writer, string value, JsonSerializer serializer)
        {
            writer.WriteValue(System.Net.WebUtility.HtmlEncode(value));
        }

        public override string ReadJson(JsonReader reader, Type objectType, string existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var str = reader.Value?.ToString() ?? string.Empty;
            return System.Net.WebUtility.HtmlEncode(str);
        }
    }
}