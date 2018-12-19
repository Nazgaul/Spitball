using System.Collections.Generic;
using Cloudents.Application.DTOs;
using Cloudents.Application.Enum;
using Cloudents.Common.Enum;
using JetBrains.Annotations;

namespace Cloudents.Application.Questions.Queries.GetQuestionsList
{
    public class QuestionWithFacetDto
    {
        public QuestionWithFacetDto()
        {
            Result = new List<QuestionFeedDto>();
        }
        [ItemCanBeNull]
        public IList<QuestionFeedDto> Result { get; set; }
        public IEnumerable<QuestionFilter> FacetState { get; set; }
        public IEnumerable<QuestionSubject> FacetSubject { get; set; }
    }
}