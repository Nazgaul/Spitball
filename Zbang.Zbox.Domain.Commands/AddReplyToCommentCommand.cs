
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{    
    public class AddReplyToCommentCommand :  ICommand
    {

        public AddReplyToCommentCommand(long userId, long commentToReplyId, string commentText, long boxId)
        {
            UserId = userId;
            CommentToReplyToId = commentToReplyId;
            CommentText = commentText;
            BoxId = boxId;
        }

        public long UserId { get; set; }

        public long CommentToReplyToId { get; set; }

        public string CommentText { get; set; }

        public long BoxId { get; set; }       
    }
}
