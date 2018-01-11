using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Newtonsoft.Json;

namespace Cloudents.Infrastructure.Search.Entities
{
    [SerializePropertyNamesAsCamelCase]
    internal class Course
    {
        [Key]
        public string Id { get; set; }

        [IsSearchable]
        [JsonProperty("Name2")]
        public string Name { get; set; }

       // [IsSearchable]
        //[JsonProperty("Professor2")]
       // public string Professor { get; set; }
        [IsSearchable]
        [JsonProperty("Course2")]

        public string Code { get; set; }

        //public string Url { get; set; }
        [IsFilterable]
        public long? UniversityId { get; set; }
        //[IsFilterable]
        //public string[] UserId { get; set; }
        //[IsSearchable]
        //public string[] Department { get; set; }

        //[IsSearchable]
        //public string[] Feed { get; set; }
        //[IsFilterable]
       // public string DepartmentId { get; set; }

        //public int? Type { get; set; }

       // public int? MembersCount { get; set; }
        //public int? ItemsCount { get; set; }

    }
}