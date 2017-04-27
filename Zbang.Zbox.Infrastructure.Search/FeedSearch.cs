using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Newtonsoft.Json;

namespace Zbang.Zbox.Infrastructure.Search
{
    [SerializePropertyNamesAsCamelCase]
    public class FeedSearch
    {
        [Key]
        public string Id { get; set; }

        [IsSearchable]
        public string Text { get; set; }

        [IsSearchable, Analyzer(AnalyzerName.AsString.EnMicrosoft)]
        [JsonProperty(FeedSearchProvider.ContentEnglishField)]
        public string TextEn { get; set; }

        [IsSearchable, Analyzer(AnalyzerName.AsString.HeMicrosoft)]
        [JsonProperty(FeedSearchProvider.ContentHebrewField)]
        public string TextHe { get; set; }

        public string Comment { get; set; }

        [IsFilterable]
        public string University { get; set; }

        [IsFilterable, IsSearchable]
        public string Course { get; set; }
        public long CourseId { get; set; }


        [IsFilterable, IsSearchable]
        public string Professor { get; set; }

        [IsFilterable]
        public string Code { get; set; }


        [IsSearchable, IsFilterable]
        public string[] Tags { get; set; }

        [IsFilterable]
        public DateTime Date { get; set; }

        [IsFilterable]
        public int? Likes { get; set; }

        [IsFilterable]
        public int? ReplyCount { get; set; }

        [IsFilterable]
        public int? ItemCount { get; set; }

        public string UserName { get; set; }
        public string UserImage { get; set; }


    }
}
