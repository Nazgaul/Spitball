using Cloudents.Admin2.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Cloudents.Admin2.Binders
{

    public abstract class JsonCreationConverter<T> : JsonConverter
    {
        public override bool CanWrite => false;

        protected abstract T Create(Type objectType, JObject jObject);

        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }


        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            if (serializer == null) throw new ArgumentNullException(nameof(serializer));
            if (reader.TokenType == JsonToken.Null)
                return null;

            var jObject = JObject.Load(reader);
            var target = Create(objectType, jObject);
            serializer.Populate(jObject.CreateReader(), target);
            return target;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public class UploadRequestJsonConverter : JsonCreationConverter<UploadRequestBase>
    {
        protected override UploadRequestBase Create(Type objectType, JObject jObject)
        {
            if (jObject == null) throw new ArgumentNullException(nameof(jObject));

            var phaseStr = jObject.GetValue("phase", StringComparison.OrdinalIgnoreCase)?.Value<string>();

            if (Enum.TryParse(phaseStr, true, out UploadPhase phase))
            {
                if (phase == UploadPhase.Start)
                {
                    return new UploadRequestStart();
                }

                if (phase == UploadPhase.Finish)
                {
                    return new UploadRequestFinish();
                }
            }

            throw new ArgumentException();
        }
    }
}
