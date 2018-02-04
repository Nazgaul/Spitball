using System;
using System.Collections.Generic;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class RemoveOldConnectionCommandHandler : ICommandHandler<RemoveOldConnectionCommand>
    {
        private readonly IConnectionRepository _connectionRepository;
         private readonly IUserRepository _userRepository;

        public RemoveOldConnectionCommandHandler(IConnectionRepository connectionRepository, IUserRepository userRepository)
        {
            _connectionRepository = connectionRepository;
            _userRepository = userRepository;
        }

        public void Handle(RemoveOldConnectionCommand message)
        {
            var zombies = _connectionRepository.GetZombies();
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

                _userRepository.Save(zombie.User);
            }
            message.UserIds = userIds;
        }
    }
}
