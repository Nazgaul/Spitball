using System;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.Security;
using System.Web.Security;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class UpdateUserPasswordCommandHandler : ICommandHandlerAsync<UpdateUserPasswordCommand, UpdateUserCommandResult>
    {
        private readonly IUserRepository m_UserRepository;
        private readonly IAccountService m_AccountService;
        private readonly UpdateUserCommandResult m_Result;

        public UpdateUserPasswordCommandHandler(IUserRepository userRepository
             , IAccountService accountService
            )
        {
            m_UserRepository = userRepository;
            m_AccountService = accountService;
            m_Result = new UpdateUserCommandResult();
        }
        public async Task<UpdateUserCommandResult> ExecuteAsync(UpdateUserPasswordCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
            User user = m_UserRepository.Load(command.Id);


            if (!user.MembershipId.HasValue)
            {
                throw new Infrastructure.Exceptions.UserNotFoundException();
            }
            await ChangeUserPasswordAsync(user.MembershipId.Value, command.CurrentPassword, command.NewPassword);

            m_UserRepository.Save(user);
            return m_Result;
        }

        private async Task ChangeUserPasswordAsync(Guid userid, string currentPassword, string newPassword)
        {
            var retVal = await m_AccountService.ChangePassword(userid, currentPassword,
                newPassword);
            if (!retVal)
            {
                m_Result.Error = Resources.CommandHandlerResources.CannotChangePwd;
            }

        }





    }
}
