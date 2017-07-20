using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Newtonsoft.Json;

namespace Zbang.Zbox.Infrastructure.Search
{
    [SerializePropertyNamesAsCamelCase]
    public class Course
    {
        [Key]
        public string Id { get; set; }

        [IsSearchable]

        public string Name { get; set; }
    }

    [SerializePropertyNamesAsCamelCase]
    public class Document
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

       // [IsFilterable]
        public string University { get; set; }

        [IsSearchable]
        public string Course { get; set; }

        [IsFilterable]
        public string UniversityId { get; set; } // because of tag

        [IsSearchable, IsFilterable]
        public string[] Tags { get; set; }

        [IsFilterable]
        public DateTime Date { get; set; }

        public string Source { get; set; }

        public string[] MetaContent { get; set; }

        [IsFilterable]
        public int? Views { get; set; }
        [IsFilterable]
        public int? Likes { get; set; }

        [IsFilterable]
        public string Domain { get; set; }

        [IsFilterable]
        public DateTime CrawlDate { get; set; }
    }


}
