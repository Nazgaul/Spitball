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
            User user = m_UserRepository.Get(command.Id);
            if (user == null)
            {
                throw new Infrastructure.Exceptions.UserNotFoundException("user doesn't exists");
            }
            user.UpdateUserProfile(command.FirstName, command.MiddleName, command.LastName, command.PicturePath, command.LargePicturePath);
            m_UserRepository.Save(user);
        }
    }
}
