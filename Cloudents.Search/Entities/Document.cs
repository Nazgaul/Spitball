using Cloudents.Common.Enum;
using Cloudents.Core.DTOs.SearchSync;
using Cloudents.Search.Interfaces;
using Microsoft.Azure.Search;
using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Search.Entities
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "json.net need public set")]
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "json.net need public set")]

    public class Document : ISearchObject
    {

        public static Document FromDto(DocumentSearchDto obj)
        {
            return new Document
            {
                UniversityId = obj.UniversityId,
                UniversityName = obj.UniversityName,
                DateTime = obj.DateTime,
                Country = obj.Country?.ToUpperInvariant(),
                Course = obj.Course?.ToUpperInvariant(),
                Id = obj.ItemId.ToString(),
                Name = obj.Name,
                Type = obj.Type,
                Tags = obj.TagsArray
            };
        }



        [System.ComponentModel.DataAnnotations.Key]
        public string Id { get; set; }
        [IsSearchable]
        public string Name { get; set; }
       // public string MetaContent { get; set; }
        [IsSearchable]
        public string Content { get; set; }

        [IsSearchable, IsFilterable]
        public string[] Tags { get; set; }
        [IsFilterable, IsSearchable, JsonProperty("Course2")]
        public string Course { get; set; }
        [IsFilterable, IsFacetable]
        public string Country { get; set; }
        [IsFilterable, IsSearchable, JsonProperty("University2")]
        public string UniversityName { get; set; }

        [IsFilterable,  JsonProperty("University")]
        public Guid UniversityId { get; set; }

        [IsSortable, IsFilterable]
        public DateTimeOffset? DateTime { get; set; }
        [IsFilterable, IsFacetable]
        public DocumentType Type { get; set; }

    }
}