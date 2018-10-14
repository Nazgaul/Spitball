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
        public QuestionsQuery(string term, int[] source, int page, IEnumerable<QuestionFilter> filters)
        {
            Term = term;
            Source = source;
            Page = page;
            Filters = filters;
        }

        public QuestionsQuery(string term, int[] source, int page, QuestionFilter filters)
        {
            Term = term;
            Source = source;
            Page = page;
            Filters = new[] { filters };
        }

        public string Term { get; }
        public int[] Source { get; }
        public int Page { get; }

        public IEnumerable<QuestionFilter> Filters { get; }
    }
}