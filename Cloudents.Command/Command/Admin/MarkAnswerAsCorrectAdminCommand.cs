using System;
using Cloudents.Application.Interfaces;

namespace Cloudents.Application.Command.Admin
{
    public class MarkAnswerAsCorrectCommand : ICommand
    {
        public MarkAnswerAsCorrectCommand(Guid answerId, long questionId)
        {
            AnswerId = answerId;
            QuestionId = questionId;
        }

        public Guid AnswerId { get; private set; }
        public long QuestionId { get; private set; }
    }
}