using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Spatial;

namespace Zbang.Zbox.Infrastructure.Search
{
    [SerializePropertyNamesAsCamelCase]
    public class Tutor : ISearchObject
    {
        [Key]
        public string Id { get; set; }

        public string Name { get; set; }
        public string Image { get; set; }
        public string Url { get; set; }
        public string City { get; set; }

        public string State { get; set; }

        [IsSortable, IsFilterable]
        public GeographyPoint Location { get; set; }
        [IsSortable]
        public double Fee { get; set; }

        [IsFilterable]
        public bool Online { get; set; }

        [IsFilterable]
        public bool InPerson { get; set; }
        [IsSearchable]
        public string[] Subjects { get; set; }


        [IsSortable]
        public double Rank { get; set; }

        [IsFilterable]
        public DateTime? InsertDate { get; set; }

        [IsSearchable]
        public string[] Extra { get; set; }
    }
}