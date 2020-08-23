using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cloudents.FunctionsV2.Models
{
    public class Course
    {
        [JsonProperty("courseName")]
        public string Name { get; set; }
        [JsonProperty("courseUrl")]
        public string Url { get; set; }

        //[JsonProperty("questions")]

        //public IEnumerable<Question> Questions { get; set; }
        [JsonProperty("documents")]

        public IEnumerable<Document> Documents { get; set; }

        [JsonProperty("extraUpdates")]
        public bool NeedMore { get; set; }
    }
}