using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class LikeReplyCommandResult : ICommandResult
    {
        public LikeReplyCommandResult(bool liked)
        {
            Liked = liked;
        }

        public bool Liked { get; private set; }
    }
}