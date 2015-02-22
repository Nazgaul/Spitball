using System;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class UpdateUserEmailCommandHandler : ICommandHandlerAsync<UpdateUserEmailCommand>
    {
        private readonly IUserRepository m_UserRepository;
        private readonly IAccountService m_AccountService;

        public UpdateUserEmailCommandHandler(IUserRepository userRepository, 
            IAccountService accountService 
            )
        {
            m_UserRepository = userRepository;
            m_AccountService = accountService;
        }

        public async Task HandleAsync(UpdateUserEmailCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
            User user = m_UserRepository.Get(command.Id);
            if (user == null)
            {
                throw new NullReferenceException(Resources.CommandHandlerResources.UserNotExist);
            }
            if (user.Email == command.Email)
            {
                return;
            }
            if (CheckIfEmailOccupied(command.Email))
            {
                throw new ArgumentException(Resources.CommandHandlerResources.EmailTaken);
            }


            if (IsChangeEmailNeeded(command.Email, user.Email))
            {
               await ChangeUserEmail(command.Email, user, command.TempFromFacebookLogin);
            }

        }

        private bool CheckIfEmailOccupied(string email)
        {
            if (m_UserRepository.GetUserByEmail(email) != null)
            {
                return true;
            }
            return false;
        }

        private bool IsChangeEmailNeeded(string newUserEmail, string currentEmail)
        {
            return !String.Equals(currentEmail, newUserEmail, StringComparison.CurrentCultureIgnoreCase);
        }

        private async Task ChangeUserEmail(string email, User user, bool tempFromFacebookLogin)
        {
            if (!Validation.IsEmailValid(email))
            {
                throw new ArgumentException(Resources.CommandHandlerResources.EmailNotCorrect);
            }
            user.UpdateEmail(email);
            if (tempFromFacebookLogin) return;
            if (user.MembershipId.HasValue)
            {
               await m_AccountService.ChangeEmail(user.MembershipId.Value, email);
                //m_MembershipService.ChangeUserEmail(user.MembershipId.Value, email);
            }
            else
            {
                throw new ArgumentException(Resources.CommandHandlerResources.CannotChangeEmail);
            }
        }
    }
}
