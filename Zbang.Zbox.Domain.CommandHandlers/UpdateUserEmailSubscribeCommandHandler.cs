using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class UpdateUserEmailSubscribeCommandHandler : ICommandHandler<UpdateUserEmailSubscribeCommand>
    {
        private readonly IUserRepository _userRepository;

        public UpdateUserEmailSubscribeCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public void Handle(UpdateUserEmailSubscribeCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var user = _userRepository.Load(message.UserId);
            if (message.SendSettings != Infrastructure.Enums.EmailSend.CanSend &&
                message.SendSettings != Infrastructure.Enums.EmailSend.Unsubscribe)
            {
                throw new ArgumentException("email settings is not valid");
            }
            user.UserTime.UpdateUserTime(user.Id);
            user.EmailSendSettings = message.SendSettings;
            _userRepository.Save(user);
        }
    }
}
