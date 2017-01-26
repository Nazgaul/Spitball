using System;
using Newtonsoft.Json;

namespace Zbang.Zbox.Infrastructure.Ai
{
    [JsonConverter(typeof(MyCustomConverter))]
    public abstract class BaseIntent
    {
        private class MyCustomConverter : JsonCreationConverter<BaseIntent>
        {
            protected override BaseIntent Create(Type objectType,
              Newtonsoft.Json.Linq.JObject jObject)
            {
                //TODO: read the raw JSON object through jObject to identify the type
                //e.g. here I'm reading a 'typename' property:
                var intent = jObject["entities"]["Intent"];
                var intentValue = intent?.First["metadata"]?.ToObject<int?>();
                if (intentValue.HasValue)
                {
                    return jObject.ToObject<KnownIntent>();

                }
                return new DontUnderstandIntent();

            }
        }
    }
}