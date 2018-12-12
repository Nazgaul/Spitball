using Cloudents.Core.Interfaces;
using System;

namespace Cloudents.Core.Answers.Commands.FlagAnswer
{
    public class FlagAnswerCommand : ICommand
    {
        public FlagAnswerCommand(long userId, Guid answerId, string flagReason)
        {
            UserId = userId;
            AnswerId = answerId;
            FlagReason = flagReason;
        }

        public long UserId { get; }
        public Guid AnswerId { get; }
        public string FlagReason { get; }
    }
}
