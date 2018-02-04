using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class UpdateUserProfileImageCommandHandler : ICommandHandler<UpdateUserProfileImageCommand>
    {
        private readonly IUserRepository _userRepository;

        public UpdateUserProfileImageCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public void Handle(UpdateUserProfileImageCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var user = _userRepository.Load(message.UserId);
            user.ImageLarge = message.ImageUrl;
            _userRepository.UpdateUserFeedDetails(user.Id);
            _userRepository.Save(user);
        }
    }
}
