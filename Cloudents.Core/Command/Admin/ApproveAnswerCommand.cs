using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.Command.Admin
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
