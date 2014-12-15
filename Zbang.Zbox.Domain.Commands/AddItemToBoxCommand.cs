using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public abstract class AddItemToBoxCommand : ICommandAsync
    {
        protected AddItemToBoxCommand(long userId, long boxId)
        {
            BoxId = boxId;
            UserId = userId;
        }

        public const string FileResolver = "File";
        public const string LinkResolver = "Link";
        public abstract string ResolverName { get; }


        public long UserId { get; private set; }

        public long BoxId { get; private set; }
    }
}
