using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Query
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Automapper initialize")]
    public class QuestionsQuery : IQuery<ResultWithFacetDto<QuestionDto>>
    {
        public string Term { get; set; }
        public string[] Source { get; set; }
        public int Page { get; set; }

        public QuestionFilter? Filter { get; set; }
    }
}