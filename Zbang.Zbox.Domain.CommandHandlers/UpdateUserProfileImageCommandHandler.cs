using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class UpdateUserProfileImageCommandHandler : ICommandHandler<UpdateUserProfileImageCommand>
    {
        private readonly IUserRepository m_UserRepository;

        public UpdateUserProfileImageCommandHandler(IUserRepository userRepository)
        {
            m_UserRepository = userRepository;
        }
        public void Handle(UpdateUserProfileImageCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var user = m_UserRepository.Load(message.UserId);
            user.ImageLarge = message.ImageUrl;
            m_UserRepository.UpdateUserFeedDetails(user.Id);
            m_UserRepository.Save(user);
        }
    }
}
