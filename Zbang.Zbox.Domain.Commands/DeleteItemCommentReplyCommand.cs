using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class DeleteItemCommentReplyCommand : ICommand, ICommandCache
    {
        public DeleteItemCommentReplyCommand(long userId, long replyId, long itemId)
        {
            ReplyId = replyId;
            ItemId = itemId;
            UserId = userId;
        }

        public long ReplyId { get; private set; }

        public long UserId { get; private set; }
        public long ItemId { get; }
        public CacheRegions CacheRegion => CacheRegions.BuildItemCommentRegion(ItemId);
    }
}
