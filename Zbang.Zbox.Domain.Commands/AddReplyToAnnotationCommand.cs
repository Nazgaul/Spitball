using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class AddReplyToAnnotationCommand : ICommand
    {
        public AddReplyToAnnotationCommand(long userId, long itemId, string comment, long itemCommentId)
        {
            UserId = userId;
            ItemId = itemId;
            Comment = comment;
            ItemCommentId = itemCommentId;
            
        }
        public long UserId { get; private set; }

        public long ItemId { get; private set; }


        public string Comment { get; private set; }

        public long ItemCommentId { get; private set; }

        public long ReplyId { get; set; }
    }
}
