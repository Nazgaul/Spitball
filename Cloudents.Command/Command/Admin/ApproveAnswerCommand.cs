using System;
using System.Collections.Generic;

namespace Cloudents.Command.Command.Admin
{
    public class ApproveAnswerCommand : ICommand
    {
        public ApproveAnswerCommand(Guid answerId)
        {
            AnswerIds = new[] { answerId };
        }

        public ApproveAnswerCommand(IEnumerable<Guid> answerIds)
        {
            AnswerIds = answerIds;
        }

        public IEnumerable<Guid> AnswerIds { get; }
    }
}
