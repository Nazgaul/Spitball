using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class UnsubscribeUsersFromEmailCommandHandler : ICommandHandler<UnsubscribeUsersFromEmailCommand>
    {
        private readonly IUserRepository m_UserRepository;

        public UnsubscribeUsersFromEmailCommandHandler(IUserRepository userRepository)
        {
            m_UserRepository = userRepository;
        }

        public void Handle(UnsubscribeUsersFromEmailCommand message)
        {
            m_UserRepository.UnsubscribeUserFromMail(message.Emails,message.Type);
        }
    }
}
