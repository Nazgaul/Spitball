using System;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.Security;

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
            if (command == null) throw new ArgumentNullException(nameof(command));
            User user = m_UserRepository.Load(command.Id);
            var id = await m_AccountService.GetUserIdAsync(user.Email);

            if (!user.MembershipId.HasValue)
            {
                user.MembershipId = id;
            }
            await ChangeUserPasswordAsync(id, command.CurrentPassword, command.NewPassword);

            m_UserRepository.Save(user);
            return m_Result;
        }

        private async Task ChangeUserPasswordAsync(Guid userid, string currentPassword, string newPassword)
        {
            var retVal = await m_AccountService.ChangePasswordAsync(userid, currentPassword,
                newPassword);
            if (!retVal)
            {
                m_Result.Error = Resources.CommandHandlerResources.CannotChangePwd;
            }
        }



    }
}
