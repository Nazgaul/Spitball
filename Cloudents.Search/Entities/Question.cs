using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Cloudents.Core.DTOs.SearchSync;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Search.Interfaces;

namespace Cloudents.Search.Entities
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global",Justification = "json.net need public set")]
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "json.net need public set")]
    public class Question : ISearchObject
    {
        //public Question(long id, DateTime dateTime, string text, string country, string language,
        //    QuestionSubject subject, QuestionFilter state)
        //{
        //    Id = id.ToString();
        //    DateTime = dateTime;
        //    Text = text;
        //    Prefix = new[] { text }.Union(subject.GetEnumLocalizationAllValues()).ToArray();
        //    Country = country.ToUpperInvariant();
        //    Language = language.ToLowerInvariant();
        //    Subject = subject;
        //    State = state;
        //}

        public Question(QuestionSearchDto dto)
        {
            Id = dto.QuestionId.ToString();
            if (dto.DateTime.HasValue && dto.DateTime.Value != System.DateTime.MinValue)
            {
                DateTime = dto.DateTime;
            }

            Text = dto.Text;
            if (dto.Text != null && dto.Subject != null)
            {
                Prefix = new[] {dto.Text}.Union(dto.Subject.GetEnumLocalizationAllValues()).ToArray();
            }

            Country = dto.Country?.ToUpperInvariant();
            Language = dto.Language?.ToLowerInvariant() ?? "en";
            if (dto.Subject.HasValue && (int) dto.Subject.Value != 0)
            {
                Subject = dto.Subject;
            }

            State = dto.Filter;
        }

        public Question()
        {
            
        }
        public string Id { get; set; } //key readonly

        public DateTime? DateTime { get;  set; } //readonly

        public string Text { get;  set; } //search readonly

        
        public string[] Prefix { get;  set; } //search



        public string Country { get;  set; }
        public string Language { get;  set; }


        public QuestionSubject? Subject { get;  set; } // facetable readonly
        public QuestionFilter? State { get; set; }

    }
}