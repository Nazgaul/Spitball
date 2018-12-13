using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.Command.Admin
{
    public class UnFlagAnswerCommand : ICommand
    {
        public UnFlagAnswerCommand(Guid answerId)
        {
            AnswerIds = new[] { answerId };
        }

        public UnFlagAnswerCommand(IEnumerable<Guid> answerIds)
        {
            AnswerIds = answerIds;
        }

        public IEnumerable<Guid> AnswerIds { get; }
    }
}
