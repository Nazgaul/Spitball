//using Cloudents.Search.Interfaces;
//using Microsoft.Azure.Search;
//using Microsoft.Azure.Search.Models;
//using Newtonsoft.Json;
//using System;
//using System.Diagnostics.CodeAnalysis;

//namespace Cloudents.Search.Entities
//{
//    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "json.net need public set")]
//    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "json.net need public set")]
//    public class University : ISearchObject
//    {

//        public const string UserCountFieldName = "UsersCount2";


//        [System.ComponentModel.DataAnnotations.Key]
//        public string Id { get; set; }
//        [IsSearchable]
//        public string Name { get; set; }
//        [IsSortable]
//        public string DisplayName { get; set; }

//        [IsSearchable]
//        public string Extra { get; set; }
//        [IsSearchable, IndexAnalyzer("prefix"), SearchAnalyzer(AnalyzerName.AsString.StandardLucene)]
//        public string[] Prefix { get; set; }
//        [IsFilterable]
//        public string Country { get; set; }

//        public string Image { get; set; }
//        [IsSortable, JsonProperty(UserCountFieldName)]
//        public int? UsersCount { get; set; }


//        [IsFilterable]
//        public DateTime? InsertDate { get; set; }

//    }
//}