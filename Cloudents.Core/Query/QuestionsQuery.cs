﻿using Cloudents.Core.Enum;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Query
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Automapper initialize")]
    public class QuestionsQuery 
    {
        public QuestionsQuery(string term, 
            QuestionSubject[] source,
            int page,
            IEnumerable<QuestionFilter> filters,
            string country
            )
        {
            Term = term;
            Country = country;
            Source = source;
            Page = page;
            Filters = filters;
        }

        public string Term { get; }
        public string Country { get; }

        public QuestionSubject[] Source { get; }
        public int Page { get; }

        public IEnumerable<QuestionFilter> Filters { get; }
    }
}