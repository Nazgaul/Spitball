﻿//using Cloudents.Core.DTOs.SearchSync;
//using Cloudents.Core.Enum;
//using Cloudents.Search.Interfaces;
//using Microsoft.Azure.Search;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Converters;
//using System;
//using System.Diagnostics.CodeAnalysis;
//using Cloudents.Core.Entities;
//using Cloudents.Infrastructure;
//using Cloudents.Search.Document;

//namespace Cloudents.Search.Entities
//{
//    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "json.net need public set")]
//    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "json.net need public set")]

//    public class Document : ISearchObject
//    {
//        public const string CourseNameField = "Course2";
//        //public const string CountryNameField = "Country2";
//        public const string TypeFieldName = "TypeFieldName";

//        public static Document FromDto(DocumentSearchDto obj)
//        {
//            return new Document
//            {


//                DateTime = obj.DateTime,
//                Country = obj.Country?.ToUpperInvariant(),
//                Course = obj.Course,
//                Id = obj.ItemId.ToString(),
//                Name = obj.Name,
//                Type = obj.Type,
//                SbCountry = obj.SbCountry
//            };
//        }



//        [System.ComponentModel.DataAnnotations.Key]
//        public string Id { get; set; }
//        [IsSearchable]
//        public string Name { get; set; }

//        [IsSearchable]
//        public string Content { get; set; }

//        [IsSearchable, IsFilterable]
//        public string[] Tags { get; set; }
//        [IsFilterable, IsSearchable, JsonProperty(CourseNameField)]
//        public string Course { get; set; }

//        [IsFilterable, IsFacetable, JsonProperty("Country"), Obsolete]
//        public string? Country { get; set; }

//        [IsFilterable, JsonConverter(typeof(CountryConverter))]
//        public Country SbCountry { get; set; }


//        [IsSortable, IsFilterable]
//        public DateTimeOffset? DateTime { get; set; }
//        [IsFilterable, IsFacetable, JsonProperty(TypeFieldName)]
//        [JsonConverter(typeof(StringEnumConverter))]
//        public DocumentType Type { get; set; }

//    }
//}