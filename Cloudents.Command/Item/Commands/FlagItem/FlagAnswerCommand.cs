using System;

namespace Cloudents.Command.Item.Commands.FlagItem
{
    public class FlagAnswerCommand : BaseFlagItemCommand<Guid>
    {
        public FlagAnswerCommand(long userId, Guid answerId, string flagReason)

        {
            Id = answerId;
            FlagReason = flagReason;
            UserId = userId;
        }
    }
}
