using System;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class UpdateUserProfileCommandHandler : ICommandHandler<UpdateUserProfileCommand>
    {
        private readonly IUserRepository _userRepository;

        public UpdateUserProfileCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public void Handle(UpdateUserProfileCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            var user = _userRepository.Load(command.Id);
            user.UpdateUserProfile(command.FirstName, command.LastName);
            _userRepository.UpdateUserFeedDetails(user.Id);
            _userRepository.Save(user);
        }
    }
}
