using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Query
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Automapper initialize")]
    public class QuestionsQuery : IQuery<IEnumerable<QuestionDto>>
    {
        public QuestionsQuery(string term, 
            
            QuestionSubject[] source,
            int page,
            IEnumerable<QuestionFilter> filters,
            string country,
            string universityId, IEnumerable<string> languages)
        {
            Term = term;
            Country = country;
            UniversityId = universityId;
            Languages = languages;
            Source = source;
            Page = page;
            Filters = filters;
        }

        public string Term { get; }
        public string Country { get; }
        public string UniversityId { get; }

        public IEnumerable<string> Languages { get; }
        public QuestionSubject[] Source { get; }
        public int Page { get; }

        public IEnumerable<QuestionFilter> Filters { get; }
    }
}