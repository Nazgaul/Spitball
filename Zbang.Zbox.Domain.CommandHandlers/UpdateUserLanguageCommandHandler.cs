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
            User user = m_UserRepository.Get(message.UserId);
            if (user == null)
            {
                throw new Infrastructure.Exceptions.UserNotFoundException("user doesn't exists");
            }
            user.UpdateUserLanguage(message.Language);
            m_UserRepository.Save(user);
        }
    }
}
