using System;
using Newtonsoft.Json;

namespace Cloudents.Web.Binders
{
    public class StringEnumNullUnknownStringConverter : Newtonsoft.Json.Converters.StringEnumConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                return base.ReadJson(reader, objectType, existingValue, serializer);
            }
            catch (JsonSerializationException)
            {
                if (objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    return null;
                }

                throw;
            }
        }
    }
}