using System;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class AddCommentCommandResult : ICommandResult
    {
        public AddCommentCommandResult(Guid commentId, string userName, string userImage, 
            //string userUrl, 
            long userId)
        {
            UserId = userId;
            //UserUrl = userUrl;
            UserImage = userImage;
            UserName = userName;
            CommentId = commentId;
        }

        public string UserName { get; }
        public string UserImage { get; }
        public Guid CommentId { get; }
        //public string UserUrl { get; }
        public long UserId { get; }

        public override string ToString()
        {
           return $"userid {UserId}  userimage {UserImage} username {UserName} commentid {CommentId}";
        }
    }
}
