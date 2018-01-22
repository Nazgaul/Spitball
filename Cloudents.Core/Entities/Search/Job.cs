using System;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Microsoft.Spatial;

namespace Cloudents.Core.Entities.Search
{
    public class Job :ISearchObject
    {
        //[Key]
        public string Id { get; set; }

        //[IsSearchable]
        public string Title { get; set; }
        public string Description { get; set; }

        //[IsSearchable]
        public string Company { get; set; }


        //[IsSearchable]
        public string City { get; set; }
        //[IsSearchable]
        public string State { get; set; }

        // [IsFilterable]
        public string Compensation { get; set; }

        //[IsSortable]
        public DateTime? DateTime { get; set; }

        // [IsFilterable, IsSortable]
        public GeographyPoint Location { get; set; }


        public string Source { get; set; }

        // [IsSearchable]
        public string[] Extra { get; set; }


        public string Url { get; set; }

        //[IsFilterable]
        public DateTime? InsertDate { get; set; }

        // [IsFilterable, IsFacetable]
        public JobFilter JobType { get; set; }
    }
}