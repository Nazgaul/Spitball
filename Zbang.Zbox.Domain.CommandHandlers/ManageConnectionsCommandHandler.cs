using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class ManageConnectionsCommandHandler : ICommandHandler<ManageConnectionsCommand>
    {
        private readonly IConnectionRepository _connectionRepository;

        public ManageConnectionsCommandHandler(IConnectionRepository connectionRepository
            )
        {
            _connectionRepository = connectionRepository;
        }

        public void Handle(ManageConnectionsCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            foreach (var connectionId in message.ConnectionIds)
            {
                var connection = _connectionRepository.Get(connectionId);
                if (connection != null)
                {
                    connection.LastActivity = DateTimeOffset.UtcNow;
                    _connectionRepository.Save(connection);
                }
            }
        }
    }
}
