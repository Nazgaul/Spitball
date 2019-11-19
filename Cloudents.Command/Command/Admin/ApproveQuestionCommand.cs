using System.Collections.Generic;

namespace Cloudents.Command.Command.Admin
{
    public class ApproveQuestionCommand : ICommand
    {
        public ApproveQuestionCommand(long questionId)
        {
            QuestionIds = new[] { questionId };
        }

        public ApproveQuestionCommand(IEnumerable<long> questionIds)
        {
            QuestionIds = questionIds;
        }

        public IEnumerable<long> QuestionIds { get; }
    }
}