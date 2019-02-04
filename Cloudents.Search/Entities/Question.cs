using Cloudents.Core.DTOs.SearchSync;
using Cloudents.Core.Enum;
using Cloudents.Search.Interfaces;
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
            Course = dto.Course;
            Subject = dto.Subject;
            State = dto.State;
            //if (dto.Subject.HasValue && dto.Subject.Value != 0)
            //{
            //    Subject = dto.Subject;
            //}

            //State = dto.Filter;
        }

        public Question(string id)
        {
            Id = id;
        }

        public Question()
        {

        }
        public string Id { get; set; } //key readonly

        public DateTime? DateTime { get; set; } //readonly

        public string Text { get; set; } //search readonly


        public string[] Prefix { get; set; } //search

        public string Course { get; set; }

        public string Country { get; set; }
        public string Language { get; set; }


        public QuestionSubject? Subject { get; set; } // facetable readonly
        public QuestionFilter? State { get; set; }

    }
}