using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Spatial;
using Newtonsoft.Json;

namespace Zbang.Zbox.Infrastructure.Search
{
    [SerializePropertyNamesAsCamelCase]
    internal class UniversitySearch
    {
        [Key]
        public string Id { get; set; }
        [IsSearchable]
        [Obsolete("use Name3")]
        public string Name { get; set; }
        [IsSearchable]
        [Obsolete("use Name3")]
        public string Name2 { get; set; }

        [IsSearchable]
        [Obsolete("use Name4")]
        public string Name3 { get; set; }

        [IsSearchable, IsSortable]
        public string Name4 { get; set; }

        [IsSearchable, IsRetrievable(false)]
        [Obsolete("use Extra3")]
        public string Extra1 { get; set; }
        [IsSearchable, IsRetrievable(false)]
        [Obsolete("use Extra3")]
        public string Extra2 { get; set; }

        [IsSearchable]
        public string[] Extra3 { get; set; }

        [JsonProperty("ImageField")]
        public string Image { get; set; }

        [JsonProperty(UniversitySearchProvider.CountryField)]
        [IsFilterable]
        public string Country { get; set; }

        [JsonProperty("membersCount")]
        public int? MembersCount { get; set; }
        [JsonProperty("membersImages")]
        public string[] MembersImages { get; set; }
        [IsSortable, IsFilterable]
        public GeographyPoint GeographyPoint { get; set; }
    }
}