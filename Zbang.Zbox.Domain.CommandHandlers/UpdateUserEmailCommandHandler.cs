using System;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class UpdateUserEmailCommandHandler : ICommandHandler<UpdateUserEmailCommand>
    {
        private readonly IUserRepository m_UserRepository;
        private readonly IMembershipService m_MembershipService;

        public UpdateUserEmailCommandHandler(IUserRepository userRepository, IMembershipService membershipService)
        {
            m_UserRepository = userRepository;
            m_MembershipService = membershipService;
        }

        public void Handle(UpdateUserEmailCommand command)
        {
            User user = m_UserRepository.Get(command.Id);
            if (user == null)
            {
                throw new NullReferenceException("user doesnt not exists");
            }
            if (user.Email == command.Email)
            {
                return;
            }
            if (CheckIfEmailOccupied(command.Email))
            {
                throw new ArgumentException("This email is taken");
            }


            if (IsChangeEmailNeeded(command.Email, user.Email))
            {
                ChangeUserEmail(command.Email, user,command.TempFromFacebookLogin);
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
            return currentEmail.ToLower() != newUserEmail.ToLower();
        }

        private void ChangeUserEmail(string email, User user, bool tempFromFacebookLogin)
        {
            if (!Validation.IsEmailValid(email))
            {
                throw new ArgumentException("Email is not in the correct format");
            }
            user.Email = email;
            if (!tempFromFacebookLogin)
            {
                if (user.MembershipId.HasValue)
                {
                    m_MembershipService.ChangeUserEmail(user.MembershipId.Value, email);
                }
                else
                {
                    throw new ArgumentException("Cannot change user email");
                }
            }
        }
    }
}
