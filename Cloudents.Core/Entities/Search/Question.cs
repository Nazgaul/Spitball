using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Cloudents.Core.Entities.Search
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global",Justification = "json.net need public set")]
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "json.net need public set")]
    public class Question : ISearchObject
    {
        public Question(long id, DateTime dateTime, string text, string country, string language,
            QuestionSubject subject, QuestionFilter state)
        {
            Id = id.ToString();
            DateTime = dateTime;
            Text = text;
            Prefix = new[] { text }.Union(subject.GetEnumLocalizationAllValues()).ToArray();
            Country = country;
            Language = language;
            Subject = subject;
            State = state;
        }

        public Question()
        {
            
        }

        public string Id { get; set; } //key readonly

        public DateTime DateTime { get;  set; } //readonly
        public string Text { get;  set; } //search readonly

        public string[] Prefix { get;  set; } //search



        public string Country { get;  set; }
        public string Language { get;  set; }


        public QuestionSubject Subject { get;  set; } // facetable readonly
        public QuestionFilter State { get; set; }


    }
}