using Cloudents.Core.DTOs.SearchSync;
using Cloudents.Core.Enum;
using Cloudents.Search.Interfaces;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Search.Entities
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "json.net need public set")]
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "json.net need public set")]
    public class Question : ISearchObject
    {
        public Question(QuestionSearchDto dto)
        {
            Id = dto.Id.ToString();
            DateTime = dto.DateTime;
            Text = dto.Text;
            Prefix = dto.Prefix;
            Country = dto.Country?.ToUpperInvariant();

            Language = dto.Language?.ToLowerInvariant();
            Course = dto.Course?.ToUpperInvariant();
            State = dto.State;
            UniversityName = dto.UniversityName;
        }

        public Question(string id)
        {
            Id = id;
        }

        public Question()
        {

        }
        [System.ComponentModel.DataAnnotations.Key]
        public string Id { get; set; } //key readonly

        [IsSortable, IsFilterable]
        public DateTime? DateTime { get; set; } //readonly
        [IsSearchable]
        public string Text { get; set; } //search readonly

        [IsSearchable, IndexAnalyzer("prefix"), SearchAnalyzer(AnalyzerName.AsString.StandardLucene)]

        public string[] Prefix { get; set; } //search

        [IsFilterable, IsFacetable, IsSearchable]
        public string Course { get; set; }

        [IsFilterable, IsSearchable]
        public string UniversityName { get; set; }

        [IsFilterable]
        public string Country { get; set; }
        [IsFilterable]
        public string Language { get; set; }

        [IsFilterable, IsFacetable]
        public QuestionFilter? State { get; set; }

        [IsFilterable, IsSearchable]
        public string[] Tags { get; set; }

    }
}