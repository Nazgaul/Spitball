using System;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local", Justification = "Automapper handle that")]
    public class MarkAnswerAsCorrectCommand : ICommand
    {
        public MarkAnswerAsCorrectCommand(Guid answerId, long userId, long questionId)
        {
            AnswerId = answerId;
            UserId = userId;
            QuestionId = questionId;
        }

        public Guid AnswerId { get; private set; }
        public long UserId { get; private set; }

        public long QuestionId { get; private set; }
    }
}