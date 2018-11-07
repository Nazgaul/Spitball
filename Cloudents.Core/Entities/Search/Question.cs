using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using System;
using System.Linq;

namespace Cloudents.Core.Entities.Search
{
    public class Question : ISearchObject
    {
        public Question(long id, DateTime dateTime, string text, string country, string language, QuestionSubject subject, QuestionFilter? state)
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

        public DateTime DateTime { get; private set; } //readonly
        public string Text { get; private set; } //search readonly

        public string[] Prefix { get; private set; } //search



        public string Country { get; private set; }
        public string Language { get; private set; }


        public QuestionSubject? Subject { get; private set; } // facetable readonly
        public QuestionFilter? State { get; set; }


    }
}