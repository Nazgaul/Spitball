using System;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command
{
    public class MarkAnswerAsCorrectCommand : ICommand
    {
        public MarkAnswerAsCorrectCommand(Guid answerId, long questionUserId)
        {
            AnswerId = answerId;
            QuestionUserId = questionUserId;
        }

        public Guid AnswerId { get; private set; }
        public long QuestionUserId { get; private set; }

    }
}