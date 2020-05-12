﻿using Cloudents.Core.DTOs.Tutors;
using Cloudents.Search.Document;
using Cloudents.Search.Interfaces;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Newtonsoft.Json;
using System;
using Cloudents.Core.Entities;

namespace Cloudents.Search.Entities
{
    public class Tutor : ISearchObject
    {
        internal const string RateFieldName = "Rate2";
        internal const string ReviewCountFieldName = "ReviewCount2";

        [System.ComponentModel.DataAnnotations.Key]
        public string Id { get; set; }

        [IsSearchable]
        public string Name { get; set; }

        [IsSortable, JsonProperty(RateFieldName), IsFilterable]
        public double Rate { get; set; }

        [IsFilterable, JsonProperty(ReviewCountFieldName)]
        public int ReviewCount { get; set; }

        [IsFilterable]
        public DateTime? InsertDate { get; set; }
        [IsSearchable]
        public string[] Courses { get; set; }

        [IsSearchable, IndexAnalyzer("prefix"), SearchAnalyzer(AnalyzerName.AsString.StandardLucene)]
        public string[] Prefix { get; set; }

        [IsSearchable]
        public string[] Subjects { get; set; }

        [IsFilterable, Obsolete]
        public string Country { get; set; }


        [IsFilterable, JsonConverter(typeof(CountryConverter))]
        public Country SbCountry { get; set; }

        [JsonConverter(typeof(StringTypeConverter))]
        public TutorCardDto Data { get; set; }

        [IsFilterable]
        public double OverAllRating { get; set; }

    }
}