using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class ManageConnectionsCommandHandler : ICommandHandler<ManageConnectionsCommand>
    {
        private readonly IConnectionRepository m_ConnectionRepository;

        public ManageConnectionsCommandHandler(IConnectionRepository connectionRepository
            )
        {
            m_ConnectionRepository = connectionRepository;
        }

        public void Handle(ManageConnectionsCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            foreach (var connectionId in message.ConnectionIds)
            {
                var connection = m_ConnectionRepository.Get(connectionId);
                if (connection != null)
                {
                    connection.LastActivity = DateTimeOffset.UtcNow;
                    m_ConnectionRepository.Save(connection);
                }
            }
        }
    }
}
