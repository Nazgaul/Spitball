using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class UpdateUserLanguageCommandHandler : ICommandHandler<UpdateUserLanguageCommand>
    {
        private readonly IUserRepository _userRepository;
        public UpdateUserLanguageCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public void Handle(UpdateUserLanguageCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var user = _userRepository.Load(message.UserId);
            user.UpdateLanguage(message.Language);
            _userRepository.Save(user);
        }
    }
}
