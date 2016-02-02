using System;

namespace Zbang.Zbox.ViewModel.Queries.QnA
{
    public class GetQuestionQuery
    {
        public GetQuestionQuery(Guid questionId, long boxId)
        {
            BoxId = boxId;
            QuestionId = questionId;
        }

        public long BoxId { get; private set; }
        public Guid QuestionId { get;private set; }
    }
}