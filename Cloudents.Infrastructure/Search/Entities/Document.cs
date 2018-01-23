using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Newtonsoft.Json;

namespace Cloudents.Infrastructure.Search.Entities
{
   // [SerializePropertyNamesAsCamelCase]
    public class Document
    {
       // [Key]
        public string Id { get; set; }
       // [IsSearchable]
        public string Name { get; set; }
        public string Image { get; set; }
        public string BoxName { get; set; }
       // [IsSearchable]
        public string Content { get; set; }
        public string MetaContent { get; set; }
        public string UniversityName { get; set; }
      //  [IsFilterable]
        public string UniversityId { get; set; }

       // [IsFilterable]
       // [JsonProperty("BoxId2")]
        public long? BoxId { get; set; }
        public string Extension { get; set; }

        public string BlobName { get; set; }
    }
}