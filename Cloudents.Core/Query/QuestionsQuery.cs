using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Query
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Automapper initialize")]
    public class QuestionsQuery : IQuery<IEnumerable<QuestionDto>>
    {
        public QuestionsQuery(string term, string[] source, int page, QuestionFilter? filter)
        {
            Term = term;
            Source = source;
            Page = page;
            Filter = filter;
        }

        public string Term { get;  }
        public string[] Source { get;  }
        public int Page { get;  }

        public QuestionFilter? Filter { get;  }
    }
}