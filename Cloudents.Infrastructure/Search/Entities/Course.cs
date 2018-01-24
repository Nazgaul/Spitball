using Newtonsoft.Json;

namespace Cloudents.Infrastructure.Search.Entities
{
   // [SerializePropertyNamesAsCamelCase]
    internal class Course
    {
      //  [Key]
        public string Id { get; set; }

       // [IsSearchable]
        [JsonProperty("Name2")]
        public string Name { get; set; }

      //  [IsSearchable]
        [JsonProperty("Course2")]

        public string Code { get; set; }

      //  [IsFilterable]
        public long? UniversityId { get; set; }

    }
}