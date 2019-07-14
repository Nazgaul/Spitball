using System.Collections.Generic;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using JetBrains.Annotations;

namespace Cloudents.Core.Questions.Queries.GetQuestionsList
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
        //public IEnumerable<QuestionSubject> FacetSubject { get; set; }
    }
}