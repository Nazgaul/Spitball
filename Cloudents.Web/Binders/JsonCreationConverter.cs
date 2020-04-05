using Cloudents.Web.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Cloudents.Web.Binders
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

    public class StringHtmlEncoderConverter : JsonConverter<string>
    {
        public override void WriteJson(JsonWriter writer, string value, JsonSerializer serializer)
        {
            writer.WriteValue(System.Net.WebUtility.HtmlEncode(value));
        }

        public override string ReadJson(JsonReader reader, Type objectType, string existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var str = reader.Value.ToString();
            return System.Net.WebUtility.HtmlEncode(str);
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
                    if (jObject["OtherUser"] != null)
                    {
                        return new FinishChatUpload();
                    }

                    return new UploadRequestFinish();
                }
            }

            throw new ArgumentException();
        }
    }
}