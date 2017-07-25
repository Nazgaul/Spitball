using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
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

        public DateTime? DateTime { get; set; }

        public string Responsibilities { get; set; }

        public string City { get; set; }
        public string State { get; set; }
        [IsFilterable]
        public string JobType { get; set; }

        [IsFilterable]
        public string CompensationType { get; set; }

        [IsFilterable, IsSortable]
        public GeographyPoint Location { get; set; }

        [IsFilterable]
        public DateTime InsertDate { get; set; }
    }

    public class JobsProvider : SearchServiceWrite<Job>
    {
        public JobsProvider(ISearchConnection connection) : base(connection, "jobs")
        {
        }

        public override Index GetIndexStructure(string indexName)
        {
            var definition = new Index
            {
                Name = indexName,
                Fields = FieldBuilder.BuildForType<Job>()
            };
            return definition;
        }
    }
}
