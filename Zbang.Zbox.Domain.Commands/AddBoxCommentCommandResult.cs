
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class AddBoxCommentCommandResult : ICommandResult
    {
        public AddBoxCommentCommandResult(Comment comment, Box target, User user)
        {
            NewComment = comment;
            Target = target;
            User = user;
        }

        public Comment NewComment { get; private set; }
        public Box Target { get; private set; }
        public User User { get; private set; }

        //public string[] GetCacheKeys()
        //{
        //    return new[] { ConstCacheKeys.Comments + Target.Id };
        //}
    }
}
