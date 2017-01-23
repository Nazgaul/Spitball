using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        [JsonProperty("content_en")]
        public string ContentEn { get; set; }

        [IsSearchable, Analyzer(AnalyzerName.AsString.HeMicrosoft)]
        [JsonProperty("content_en")]
        public string ContentHe { get; set; }

        [IsFacetable,IsFilterable]
        public string University { get; set; }

        [IsFacetable, IsFilterable]
        public string Course { get; set; }

        [IsFacetable, IsFilterable]
        public string Professor { get; set; }

        [IsFacetable, IsFilterable]
        public string Code { get; set; }

        [IsFacetable, IsFilterable, IsSortable]
        public string Type { get; set; }

        [IsSearchable, IsFacetable, IsFilterable]
        public string[] Tags { get; set; }
    }
}
