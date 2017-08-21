using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace Zbang.Zbox.Infrastructure.Search
{
    [SerializePropertyNamesAsCamelCase]
    internal class BoxSearch
    {
        [Key]
        public string Id { get; set; }

        [IsSearchable]
        [Obsolete("use Name2")]
        public string Name { get; set; }

        [IsSearchable]
        public string Name2 { get; set; }

        [IsSearchable]
        [Obsolete("use Professor2")]
        public string Professor { get; set; }
        [IsSearchable]
        [Obsolete("use Course2")]
        public string Course { get; set; }

        [IsSearchable]
        public string Professor2 { get; set; }
        [IsSearchable]
        public string Course2 { get; set; }

        public string Url { get; set; }
        [IsFilterable]
        public long? UniversityId { get; set; }
        [IsFilterable]
        public string[] UserId { get; set; }
        [IsSearchable]
        public string[] Department { get; set; }

        [IsSearchable]
        public string[] Feed { get; set; }
        [IsFilterable]
        public string DepartmentId { get; set; }

        public int? Type { get; set; }

        public int? MembersCount { get; set; }
        public int? ItemsCount { get; set; }

        [Obsolete]
        [IsSearchable,IsSortable]
        public string ParentDepartment { get; set; }

    }
}