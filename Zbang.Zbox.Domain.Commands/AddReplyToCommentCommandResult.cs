using System.Runtime.Serialization;
using Zbang.Zbox.Infrastructure.Commands;
using Zbang.Zbox.Infrastructure.Cache;

namespace Zbang.Zbox.Domain.Commands
{
    [DataContract]
    public class AddReplyToCommentCommandResult : ICommandResult
    {
        public AddReplyToCommentCommandResult(CommentReplies comment, User user, long boxId)
        {
            NewComment = comment;
            User = user;
            BoxId = boxId;
        }

        public CommentReplies NewComment { get; private set; }
        public User User { get; private set; }
        private long BoxId { get; set; }


        //public string[] GetCacheKeys()
        //{
        //    return new[] { ConstCacheKeys.Comments + BoxId };
        //}
    }
}
