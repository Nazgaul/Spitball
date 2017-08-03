using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Spatial;

namespace Zbang.Zbox.Infrastructure.Search
{
    [SerializePropertyNamesAsCamelCase]
    public class Job : ISearchObject
    {
        [Key]
        public string Id { get; set; }

        [IsSearchable]
        public string Title { get; set; }

        [IsSortable]
        public DateTime? DateTime { get; set; }

        public string Responsibilities { get; set; }

        public string City { get; set; }
        public string State { get; set; }
        [IsFilterable, IsFacetable]
        public string JobType { get; set; }

        [IsFilterable]
        public string CompensationType { get; set; }

        [IsFilterable, IsSortable]
        public GeographyPoint Location { get; set; }

        [IsFilterable]
        public DateTime? InsertDate { get; set; }

        public string Url { get; set; }

        public string Company { get; set; }

        //public string LocationType { get; set; }
    }
}