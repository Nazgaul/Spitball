using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Enum;
using Cloudents.Core.Models;
using JetBrains.Annotations;

namespace Cloudents.Core.Query
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Automapper initialize")]
    public class QuestionsQuery : VerticalQuery
    {
        public QuestionsQuery(UserProfile userProfile, 
            string term,
            string course, 
            bool filterByUniversity,
            QuestionSubject[] source,
            IEnumerable<QuestionFilter> filters) : base(userProfile, term, course, filterByUniversity)
        {
            Source = source;
            Filters = filters;
        }


        public QuestionSubject[] Source { get; }

        public IEnumerable<QuestionFilter> Filters { get; }
    }
}