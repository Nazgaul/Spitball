using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class ChangeUserOnlineStatusCommand : ICommand
    {
        public ChangeUserOnlineStatusCommand(long userId, bool online, string connectionId)
        {
            UserId = userId;
            Online = online;
            ConnectionId = connectionId;
        }

        public long UserId { get; private set; }

        public bool Online { get; private set; }

        public string ConnectionId { get;private set; }
    }
}
