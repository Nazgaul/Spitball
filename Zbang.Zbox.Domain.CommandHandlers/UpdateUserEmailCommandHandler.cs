using System;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Mail;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class UpdateUserEmailCommandHandler : ICommandHandlerAsync<UpdateUserEmailCommand>
    {
        private readonly IUserRepository m_UserRepository;
        private readonly IAccountService m_AccountService;
        private readonly IEmailVerification m_EmailVerification;

        public UpdateUserEmailCommandHandler(IUserRepository userRepository,
            IAccountService accountService, IEmailVerification emailVerification)
        {
            m_UserRepository = userRepository;
            m_AccountService = accountService;
            m_EmailVerification = emailVerification;
        }

        public async Task HandleAsync(UpdateUserEmailCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            var user = m_UserRepository.Load(command.Id);
            if (!IsChangeEmailNeeded(command.Email, user.Email))
            {
                return;
            }
            if (CheckIfEmailOccupied(command.Email))
            {
                throw new ArgumentException(Resources.CommandHandlerResources.EmailTaken);
            }

            await ChangeUserEmailAsync(command.Email, user);

        }

        private bool CheckIfEmailOccupied(string email)
        {
            if (m_UserRepository.GetUserByEmail(email) != null)
            {
                return true;
            }
            return false;
        }

        private static bool IsChangeEmailNeeded(string newUserEmail, string currentEmail)
        {
            return !string.Equals(currentEmail, newUserEmail, StringComparison.CurrentCultureIgnoreCase);
        }

        private async Task ChangeUserEmailAsync(string email, User user)
        {
            if (!await m_EmailVerification.VerifyEmailAsync(email))
            //if (!Validation.IsEmailValid(email))
            {
                throw new ArgumentException(Resources.CommandHandlerResources.EmailNotCorrect);
            }
            user.UpdateEmail(email);
            if (user.MembershipId.HasValue)
            {
                await m_AccountService.ChangeEmailAsync(user.MembershipId.Value, email);
            }
        }
    }
}
