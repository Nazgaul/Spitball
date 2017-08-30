using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class UpdateUserLanguageCommandHandler : ICommandHandler<UpdateUserLanguageCommand>
    {
        private readonly IUserRepository m_UserRepository;
        public UpdateUserLanguageCommandHandler(IUserRepository userRepository)
        {
            m_UserRepository = userRepository;
        }
        public void Handle(UpdateUserLanguageCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var user = m_UserRepository.Load(message.UserId);
            user.UpdateLanguage(message.Language);
            m_UserRepository.Save(user);
        }
    }
}
