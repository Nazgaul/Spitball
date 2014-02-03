using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    class VerifyEmailCommandHandler: ICommandHandler<VerifyEmailCommand, VerifyEmailCommandResult>
    {
        //Fields
        private IUserRepository m_UserRepository;
        private const long GIGA_IN_BYTES = 10737418240;

        //Ctor
        public VerifyEmailCommandHandler(IUserRepository userRepository)
        {
            m_UserRepository = userRepository;
        }

        //Methods
        public VerifyEmailCommandResult Execute(VerifyEmailCommand command)
        {
            VerifyEmailCommandResult result = new VerifyEmailCommandResult();

            User user = m_UserRepository.Get(command.Id);

            if (user == null)
            {
                result.Verified = false;
            }
            else
            {
                user.IsEmailVerified = true;
                user.UserQuota.AllocatedSize = GIGA_IN_BYTES;

                m_UserRepository.Save(user);

                result.Verified = true;
            }                
            return result;
        }
    }
}
