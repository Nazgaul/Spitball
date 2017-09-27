using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class AddItemCommentCommand : ICommandAsync, ICommandCache
    {
        public AddItemCommentCommand(string comment,  long itemId,  long userId, long boxId)
        {
            BoxId = boxId;
            Comment = comment;
            ItemId = itemId;
            UserId = userId;
        }

        public string Comment { get; private set; }

      
        public long ItemId { get; private set; }

        public long UserId { get; private set; }

        //out parameter
        public long CommentId { get; set; }

        public long BoxId { get; private set; }
        public CacheRegions CacheRegion => CacheRegions.BuildItemCommentRegion(ItemId);
    }
}
