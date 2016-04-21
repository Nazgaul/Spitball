using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class UpdateUserThemeCommandHandler : ICommandHandler<UpdateUserThemeCommand>
    {
        private readonly IUserRepository m_UserRepository;

        public UpdateUserThemeCommandHandler(IUserRepository userRepository)
        {
            m_UserRepository = userRepository;
        }
        public void Handle(UpdateUserThemeCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var user = m_UserRepository.Load(message.Id);
            user.Theme = message.Theme;
            m_UserRepository.Save(user);
        }
    }
}
