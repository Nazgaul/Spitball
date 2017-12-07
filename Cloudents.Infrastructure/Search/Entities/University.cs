using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Spatial;
using Newtonsoft.Json;

namespace Cloudents.Infrastructure.Search.Entities
{
    [SerializePropertyNamesAsCamelCase]
    internal class University
    {
        public const string NameProperty = "name4";

        [Key]
        public string Id { get; set; }

        [IsSearchable]
        [JsonProperty(NameProperty)]
        public string Name { get; set; }

        [IsSearchable]
        public string[] Extra3 { get; set; }

        [JsonProperty("ImageField")]
        public string Image { get; set; }

        // ReSharper disable once StringLiteralTypo this is what it is :(
        [JsonProperty("coutry")]
        [IsFilterable]
        public string Country { get; set; }

        //[JsonProperty("membersCount")]
        //public int? MembersCount { get; set; }
        //[JsonProperty("membersImages")]
        //public string[] MembersImages { get; set; }
        [IsSortable,IsFilterable]
        public GeographyPoint GeographyPoint { get; set; }
    }
}