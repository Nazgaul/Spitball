using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class UpdateUserEmailSubscribeCommandHandler : ICommandHandler<UpdateUserEmailSubscribeCommand>
    {
        private readonly IUserRepository m_UserRepository;

        public UpdateUserEmailSubscribeCommandHandler(IUserRepository userRepository)
        {
            m_UserRepository = userRepository;
        }

        public void Handle(UpdateUserEmailSubscribeCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var user = m_UserRepository.Load(message.UserId);
            if (message.SendSettings != Infrastructure.Enums.EmailSend.CanSend &&
                message.SendSettings != Infrastructure.Enums.EmailSend.Unsubscribe)
            {
                throw new ArgumentException("email settings is not valid");
            };
            user.EmailSendSettings = message.SendSettings;
            m_UserRepository.Save(user);
        }
    }
}
