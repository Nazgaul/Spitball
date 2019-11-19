using Cloudents.Core.Extension;
using Newtonsoft.Json;
using System;

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

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            Enum e = (Enum)value;
            e.GetEnumLocalization();
            writer.WriteValue(e.GetEnumLocalization());

        }
    }
}