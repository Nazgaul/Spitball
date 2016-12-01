using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class DeleteItemCommentCommand : ICommand, ICommandCache
    {
        public DeleteItemCommentCommand(long itemCommentId, long userId, long itemId)
        {
            ItemCommentId = itemCommentId;
            UserId = userId;
            ItemId = itemId;
        }
        public long ItemCommentId { get; set; }

        public long UserId { get; set; }

        public long ItemId { get; private set; }
        public string CacheRegion => CacheRegions.BuildItemCommentRegion(ItemId);
    }
}
