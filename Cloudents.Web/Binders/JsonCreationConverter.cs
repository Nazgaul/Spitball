using System;
using Cloudents.Web.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cloudents.Web.Binders
{
    public abstract class JsonCreationConverter<T> : JsonConverter
    {
        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        protected abstract T Create(Type objectType, JObject jObject);

        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }


        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader == null) throw new ArgumentNullException("reader");
            if (serializer == null) throw new ArgumentNullException("serializer");
            if (reader.TokenType == JsonToken.Null)
                return null;

            JObject jObject = JObject.Load(reader);
            T target = Create(objectType, jObject);
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
            if (jObject == null) throw new ArgumentNullException("jObject");

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
            //if (jObject["Phase"] != null)
            //{
            //    return new UploadRequestStart();
            //}
            //else if (jObject["hospitalName"] != null)
            //{
            //    return new Doctor();
            //}
            //else
            //{
            //    return new Person();
            //}
        }
    }
}