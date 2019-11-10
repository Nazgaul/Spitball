using Microsoft.AspNetCore.Localization;
using Newtonsoft.Json;
using System;

namespace Cloudents.Web.Binders
{
    public class RequestCultureConverter : JsonConverter<RequestCulture>
    {

        public override void WriteJson(JsonWriter writer, RequestCulture value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanWrite => false;

        public override RequestCulture ReadJson(JsonReader reader, Type objectType, RequestCulture existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var s = reader.Value.ToString();
            if (string.IsNullOrEmpty(s))
            {
                return null;
            }
            return new RequestCulture(s);
        }
    }
}