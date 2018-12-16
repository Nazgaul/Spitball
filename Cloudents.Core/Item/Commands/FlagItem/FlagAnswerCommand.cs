using System;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Item.Commands.FlagItem
{
    public class FlagAnswerCommand : BaseFlagItemCommand<Guid>
    {
        public FlagAnswerCommand(long userId, Guid answerId, string flagReason)
            : base(userId, answerId, flagReason)
        {

        }
    }

    public class BaseFlagItemCommand<T> : ICommand
    {
        protected BaseFlagItemCommand(long userId, T id, string flagReason)
        {
            UserId = userId;
            Id = id;
            FlagReason = flagReason;
        }

        public long UserId { get; }
        public T Id { get; }
        public string FlagReason { get; }
    }
}
