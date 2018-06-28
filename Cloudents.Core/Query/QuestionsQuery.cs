using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Query
{
    public class QuestionsQuery : IQuery<ResultWithFacetDto<QuestionDto>>
    {
        public string Term { get; set; }
        public string[] Source { get; set; }
        public int Page { get; set; }
    }
}