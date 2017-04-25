using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Newtonsoft.Json;

namespace Zbang.Zbox.Infrastructure.Search
{
    [SerializePropertyNamesAsCamelCase]
    public class Item
    {
        [Key]
        public string Id { get; set; }
        [IsSearchable]
        public string Name { get; set; }

        [IsSearchable]
        public string Content { get; set; }

        [IsSearchable, Analyzer(AnalyzerName.AsString.EnMicrosoft)]
        [JsonProperty(ContentSearchProvider.ContentEnglishField)]
        public string ContentEn { get; set; }

        [IsSearchable, Analyzer(AnalyzerName.AsString.HeMicrosoft)]
        [JsonProperty(ContentSearchProvider.ContentHebrewField)]
        public string ContentHe { get; set; }

        [IsFilterable]
        public string University { get; set; }

        [IsFilterable,IsSearchable]
        public string Course { get; set; }

        public long CourseId { get; set; }

        [IsFilterable, IsSearchable]
        public string Professor { get; set; }

        [IsFilterable]
        public string Code { get; set; }

        [IsFacetable, IsFilterable]
        public string[] Type { get; set; }

        [IsSearchable, IsFilterable]
        public string[] Tags { get; set; }

        [IsFilterable]
        public DateTime Date { get; set; }

        public string BlobName { get; set; }

        public string[] MetaContent { get; set; }

        [IsFilterable]
        public int? Views { get; set; }
        [IsFilterable]
        public int? Likes { get; set; }
        public int? ContentCount { get; set; }
    }


}
