using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Infrastructure.Ai
{
    [JsonConverter(typeof(MyCustomConverter))]
    public class SearchIntent : BaseIntent
    {
        private class MyCustomConverter : JsonCreationConverter<SearchIntent>
        {
            protected override SearchIntent Create(Type objectType,
              JObject jObject)
            {
                if (jObject == null) throw new ArgumentNullException(nameof(jObject));
                //TODO: read the raw JSON object through jObject to identify the type
                //e.g. here I'm reading a 'typename' property:
                var intent = jObject["entities"]["SearchType"];
                var intentValue = intent.First["metadata"].ToObject<int>();
                if (intentValue == 7)
                {
                    return new SearchAllDocuments();
                }
                return new SearchHomework();
               
            }
        }

        // public override string ToString()
        // {
        //     var listOfStr = new List<string> {"You wanted to search"};


        //     if (string.IsNullOrEmpty(SearchType))
        //     {
        //         listOfStr.Add("I don't know the search type");
        //     }
        //     else
        //     {
        //         listOfStr.Add("The type of search is" + SearchType);
        //     }
        //     if (string.IsNullOrEmpty(University))
        //     {
        //         listOfStr.Add("I don't know the university");
        //     }
        //     else
        //     {
        //         listOfStr.Add("In university " + University);
        //     }
        //     if (string.IsNullOrEmpty(Course))
        //     {
        //         listOfStr.Add("I don't know which class");
        //     }
        //     else
        //     {
        //         listOfStr.Add("In Class " + Course);
        //     }
        //     if (string.IsNullOrEmpty(Term))
        //     {
        //         listOfStr.Add("I don't know the term");
        //     }
        //     else
        //     {
        //         listOfStr.Add("The term is " + Term);
        //     }
        //     return string.Join(". ", listOfStr);
        // }
    }

    public class SearchAllDocuments : SearchIntent
    {
        
    }

    public class SearchHomework : SearchIntent
    {
        
    }
}
