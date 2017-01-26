using System;
using Newtonsoft.Json;

namespace Zbang.Zbox.Infrastructure.Ai
{
    [JsonConverter(typeof(MyCustomConverter))]
    public abstract class BaseIntent : IIntent
    {
        [JsonProperty("entities.search_query[0].value")]
        public string Term { get; set; }

        [JsonProperty("entities.UniversityName[0].value")]
        public string University { get; set; }

        //[WitApiName("CourseName")]
        [JsonProperty("entities.CourseName[0].value")]
        public string Course { get; set; }

        private class MyCustomConverter : JsonCreationConverter<BaseIntent>
        {
            protected override BaseIntent Create(Type objectType,
              Newtonsoft.Json.Linq.JObject jObject)
            {
                //TODO: read the raw JSON object through jObject to identify the type
                //e.g. here I'm reading a 'typename' property:
                var intent = jObject["entities"]["Intent"];
                var intentValue = intent.First["metadata"].ToObject<int?>();
                
                if (intentValue == 2)
                {
                    return jObject.ToObject<SearchIntent>();
                }
                if (intentValue == 1)
                {
                    return jObject.ToObject<QuestionIntent>();
                }
                return new DontUnderstandIntent();
               
            }
        }
    }
}