using System;

namespace Cloudents.Application.Item.Commands.FlagItem
{
    public class FlagAnswerCommand : BaseFlagItemCommand<Guid>
    {
        public FlagAnswerCommand(long userId, Guid answerId, string flagReason)
            
        {
            Id = answerId;
            FlagReason = flagReason;
            UserId = userId;
        }

        internal FlagAnswerCommand(Guid answerId)
        {
            Id = answerId;
            FlagReason = "Too many down vote";
        }

    }
}
