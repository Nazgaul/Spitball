using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class UpdateUserMobileSettings : ICommandHandler<RegisterMobileDeviceCommand>
    {
        private readonly IUserRepository m_UserRepository;

        public UpdateUserMobileSettings(IUserRepository userRepository)
        {
            m_UserRepository = userRepository;
        }

        public void Handle(RegisterMobileDeviceCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var user = m_UserRepository.Load(message.UserId);
            user.MobileDevice = message.OperatingSystem;
            user.LastAccessTime = DateTime.UtcNow;
            m_UserRepository.Save(user);
        }
    }
}