using System;
using Cloudents.Core.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cloudents.Web.Binders
{
    public class CountryConverter : JsonConverter<Country>
    {
        public override void WriteJson(JsonWriter writer, Country value, JsonSerializer serializer)
        {
            JToken t = JToken.FromObject(value.ToString());
            t.WriteTo(writer);


        }

        public override Country ReadJson(JsonReader reader, Type objectType, Country existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}