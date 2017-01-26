using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Infrastructure.Ai
{
    [JsonConverter(typeof(MyCustomConverter))]
    public class SearchDocumentIntent : SearchIntent
    {
        public virtual ItemType? TypeToSearch => null;

        public override SearchType SearchType => SearchType.Document;

        private class MyCustomConverter : JsonCreationConverter<SearchDocumentIntent>
        {
            protected override SearchDocumentIntent Create(Type objectType,
                JObject jObject)
            {
                if (jObject == null) throw new ArgumentNullException(nameof(jObject));
                //TODO: read the raw JSON object through jObject to identify the type
                var intent = jObject["entities"]["SearchType"];
                var intentValue = intent?.First["metadata"]?.ToObject<int?>();
                
                if (intentValue == 16)
                {
                    return new SearchHomework();
                }
                if (intentValue == 32)
                {
                    return new SearchLecture();
                }

                if (intentValue == 64)
                {
                    return new SearchStudyGuide();
                }


                if (intentValue == 2)
                {
                    return new SearchQuiz();
                }


                if (intentValue == 256)
                {
                    return new SearchClassNote();
                }


                if (intentValue == 1)
                {
                    return new SearchFlashcard();
                }
                if (intentValue == 7)
                {
                    return new SearchAllDocuments();
                }
                return new SearchDocumentIntent();



            }
        }
    }
}