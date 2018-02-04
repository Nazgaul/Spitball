using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class UnsubscribeUsersFromEmailCommandHandler : ICommandHandler<UnsubscribeUsersFromEmailCommand>
    {
        private readonly IUserRepository _userRepository;

        public UnsubscribeUsersFromEmailCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public void Handle(UnsubscribeUsersFromEmailCommand message)
        {
            _userRepository.UnsubscribeUserFromMail(message.Emails,message.Type);
        }
    }
}
