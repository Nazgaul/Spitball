using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Enum;

namespace Cloudents.Core.Query
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Automapper initialize")]
    public class QuestionsQuery //: IQuery<IEnumerable<QuestionDto>>
    {
        public QuestionsQuery(string term, string[] source, int page, IEnumerable<QuestionFilter> filters)
        {
            Term = term;
            Source = source;
            Page = page;
            Filters = filters;
        }

        public string Term { get;  }
        public string[] Source { get;  }
        public int Page { get;  }

        public IEnumerable<QuestionFilter> Filters { get;  }
    }
}