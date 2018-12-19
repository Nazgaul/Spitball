using System.Collections.Generic;
using Cloudents.Application.Interfaces;

namespace Cloudents.Application.Command.Admin
{
    public class ApproveQuestionCommand : ICommand
    {
        public ApproveQuestionCommand(long questionId)
        {
            QuestionIds = new [] { questionId };
        }

        public ApproveQuestionCommand(IEnumerable<long> questionIds)
        {
            QuestionIds = questionIds;
        }

        public IEnumerable<long> QuestionIds { get; }
    }
}