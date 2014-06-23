using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.Security;
using System.Web.Security;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class UpdateUserPasswordCommandHandler : ICommandHandler<UpdateUserPasswordCommand, UpdateUserCommandResult>
    {
        private readonly IUserRepository m_UserRepository;
        private readonly IMembershipService m_MembershipService;
        private readonly UpdateUserCommandResult m_Result;

        public UpdateUserPasswordCommandHandler(IUserRepository userRepository, IMembershipService membershipService)
        {
            m_UserRepository = userRepository;
            m_MembershipService = membershipService;
            m_Result = new UpdateUserCommandResult();
        }
        public UpdateUserCommandResult Execute(UpdateUserPasswordCommand command)
        {
            User user = m_UserRepository.Get(command.Id);
            if (user == null)
                throw new NullReferenceException("user doesnt not exists");


            if (IsUserRegisteredLocaly(user))
            {
                ChangeUserPassword(command, user);
            }
            else
            {
                RegisterUserLocally(command, user);
            }

           
              
           
            m_UserRepository.Save(user);
            return m_Result;            
        }

        private void ChangeUserPassword(UpdateUserPasswordCommand command, User user)
        {
            if (!m_MembershipService.ChangePassword(user.MembershipId.Value, command.CurrentPassword, command.NewPassword))
            {
                m_Result.Error = Resources.CommandHandlerResources.CannotChangePwd;
            }
        }

        private void RegisterUserLocally(UpdateUserPasswordCommand command, User user)
        {
            Guid memberShipUserProviderName;
            var creationStatus = m_MembershipService.CreateUser(Guid.NewGuid().ToString(), command.NewPassword, user.Email, out memberShipUserProviderName);
            if (creationStatus == MembershipCreateStatus.Success)
            {
                user.MembershipId = memberShipUserProviderName;
            }
            else
            {
                m_Result.Error = AccountValidation.ErrorCodeToString(creationStatus);
            }
        }

        private bool IsUserRegisteredLocaly(User user)
        {
            return user.MembershipId.HasValue;
        }

    }
}
