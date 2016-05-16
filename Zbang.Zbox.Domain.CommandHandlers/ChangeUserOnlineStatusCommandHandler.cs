using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class ChangeUserOnlineStatusCommandHandler : ICommandHandler<ChangeUserOnlineStatusCommand>
    {
        private readonly IUserRepository m_UserRepository;

        public ChangeUserOnlineStatusCommandHandler(IUserRepository userRepository)
        {
            m_UserRepository = userRepository;
        }

        public void Handle(ChangeUserOnlineStatusCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var user = m_UserRepository.Load(message.UserId);
            user.Online = message.Online;
            user.LastAccessTime = DateTime.UtcNow;
            m_UserRepository.Save(user);
        }
    }
}
