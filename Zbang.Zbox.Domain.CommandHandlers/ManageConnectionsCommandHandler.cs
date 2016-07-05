using System;
using System.Collections.Generic;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class ManageConnectionsCommandHandler : ICommandHandler<ManageConnectionsCommand, ManageConnectionsCommandResult>
    {
        private readonly IConnectionRepository m_ConnectionRepository;
        private readonly IUserRepository m_UserRepository;

        public ManageConnectionsCommandHandler(IConnectionRepository connectionRepository, IUserRepository userRepository)
        {
            m_ConnectionRepository = connectionRepository;
            m_UserRepository = userRepository;
        }

        public ManageConnectionsCommandResult Execute(ManageConnectionsCommand message)
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
            var zombies = m_ConnectionRepository.GetZombies();
            TraceLog.WriteInfo($"zombies count {zombies.Count}");
            var userIds = new List<long>();
            //foreach (var zombie in zombies)
            //{
            //    var user = zombie.User;

            //    if (user.Connections.Count == 1)
            //    {
            //        user.Online = false;
            //        user.LastAccessTime = DateTime.UtcNow;
            //        userIds.Add(user.Id);
            //        user.Connections.Clear();

            //    }
            //    else
            //    {
            //        user.Connections.Remove(zombie);
            //    }

            //    m_UserRepository.Save(zombie.User);
            //}
            return new ManageConnectionsCommandResult(userIds);
        }
    }
}
