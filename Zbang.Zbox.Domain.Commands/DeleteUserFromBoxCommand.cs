using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class DeleteUserFromBoxCommand : ICommand
    {
        public DeleteUserFromBoxCommand(long userId, long userToDeleteId, long boxId)
        {
            UserId = userId;
            UserToDeleteId = userToDeleteId;
            BoxId = boxId;

        }

        public long UserId { get; private set; }

        public long UserToDeleteId { get; private set; }

        public long BoxId { get; private set; }

    }
}
