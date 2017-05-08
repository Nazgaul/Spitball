using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class CreateJaredUserCommandHandler : ICommandHandler<CreateJaredUserCommand, CreateJaredUserCommandResult>
    {
        private readonly IUserRepository m_UserRepository;

        public CreateJaredUserCommandHandler(IUserRepository userRepository)
        {
            m_UserRepository = userRepository;
        }

        public CreateJaredUserCommandResult Execute(CreateJaredUserCommand message)
        {
            var user = m_UserRepository.GetUserByEmail(message.UserIdToken.ToString());
            if (user != null)
            {
                return new CreateJaredUserCommandResult(user.Id);
            }
            user = new User(message.UserIdToken.ToString(), null, "John", "Doe", null,
                Infrastructure.Enums.Sex.NotKnown)
            {
                UserType = Infrastructure.Enums.UserType.Jared,
                IsRegisterUser = true,
                EmailSendSettings = Infrastructure.Enums.EmailSend.Invalid
            };
            user.Quota.AllocateStorage();
            m_UserRepository.Save(user, true);
            return new CreateJaredUserCommandResult(user.Id);
        }
    }
}
