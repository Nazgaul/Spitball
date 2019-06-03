using System;
using Cloudents.Search.Interfaces;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace Cloudents.Search.Entities
{
    public class Tutor : ISearchObject
    {
        [System.ComponentModel.DataAnnotations.Key]
        public string Id { get; set; }

        [IsSearchable]
        public string Name { get; set; }
        [IsSearchable]
        public string Bio { get; set; }
        public string Image { get; set; }
        public double Price { get; set; }
        public double Rate { get; set; }
        public int ReviewCount { get; set; }

        [IsFilterable]
        public DateTime? InsertDate { get; set; }
        [IsSearchable]
        public string[] Courses { get; set; }

        [IsSearchable, IndexAnalyzer("prefix"), SearchAnalyzer(AnalyzerName.AsString.StandardLucene)]
        public string[] Prefix { get; set; }

        [IsSearchable]
        public string[] Subjects { get; set; }
        [IsFilterable]
        public string Country { get; set; }

    }
}