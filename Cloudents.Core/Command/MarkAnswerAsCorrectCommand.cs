using System;
using Cloudents.Application.Interfaces;

namespace Cloudents.Application.Command
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