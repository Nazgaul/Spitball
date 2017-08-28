using System;
using System.Collections.Generic;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class RemoveOldConnectionCommandHandler : ICommandHandler<RemoveOldConnectionCommand>
    {
        private readonly IConnectionRepository m_ConnectionRepository;
         private readonly IUserRepository m_UserRepository;

        public RemoveOldConnectionCommandHandler(IConnectionRepository connectionRepository, IUserRepository userRepository)
        {
            m_ConnectionRepository = connectionRepository;
            m_UserRepository = userRepository;
        }

        public void Handle(RemoveOldConnectionCommand message)
        {
            var zombies = m_ConnectionRepository.GetZombies();
            if (zombies.Count == 0)
            {
                return;
            }
            var userIds = new List<long>();
            foreach (var zombie in zombies)
            {
                var user = zombie.User;

                if (user.Connections.Count == 1)
                {
                    user.Online = false;
                    user.LastAccessTime = DateTime.UtcNow;
                    userIds.Add(user.Id);
                    user.Connections.Clear();
                }
                else
                {
                    user.Connections.Remove(zombie);
                }

                m_UserRepository.Save(zombie.User);
            }
            message.UserIds = userIds;
        }
    }
}
