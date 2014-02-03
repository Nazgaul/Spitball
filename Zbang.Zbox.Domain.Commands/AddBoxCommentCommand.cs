using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class AddBoxCommentCommand :  ICommand
    {
        public AddBoxCommentCommand(long userId, long boxId, string commentText)
        {
            UserId = userId;
            BoxId = boxId;
            CommentText = commentText;
        }

        public long UserId { get; private set; }

        public long BoxId { get; private set; }

        public string CommentText { get; private set; }     
    }
}
