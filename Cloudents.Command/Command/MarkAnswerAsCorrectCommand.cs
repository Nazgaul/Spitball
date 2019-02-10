using System;

namespace Cloudents.Command.Command
{
    public class MarkAnswerAsCorrectCommand : ICommand
    {
        public MarkAnswerAsCorrectCommand(Guid answerId, long questionUserId)
        {
            AnswerId = answerId;
            QuestionUserId = questionUserId;
        }

        public Guid AnswerId { get; }
        public long QuestionUserId { get; }

    }
}