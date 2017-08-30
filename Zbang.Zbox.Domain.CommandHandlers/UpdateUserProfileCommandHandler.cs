using System;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class UpdateUserProfileCommandHandler : ICommandHandler<UpdateUserProfileCommand>
    {
        private readonly IUserRepository m_UserRepository;

        public UpdateUserProfileCommandHandler(IUserRepository userRepository)
        {
            m_UserRepository = userRepository;
        }
        public void Handle(UpdateUserProfileCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            var user = m_UserRepository.Load(command.Id);
            user.UpdateUserProfile(command.FirstName, command.LastName);
            m_UserRepository.UpdateUserFeedDetails(user.Id);
            m_UserRepository.Save(user);
        }
    }
}
