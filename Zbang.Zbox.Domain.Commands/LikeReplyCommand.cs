using System;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class LikeReplyCommand : ICommand
    {
        public LikeReplyCommand(Guid replyId, long userId)
        {
            UserId = userId;
            ReplyId = replyId;
        }

        public long UserId { get; private set; }
        public Guid ReplyId { get; private set; }
    }
}
