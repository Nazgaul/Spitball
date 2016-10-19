using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class UpdateQuotaCommandHandler : ICommandHandler<UpdateQuotaCommand>
    {
        private readonly IUserRepository m_UserRepository;

        public UpdateQuotaCommandHandler( IUserRepository userRepository)
        {
            m_UserRepository = userRepository;
        }

        public void Handle(UpdateQuotaCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (message.UserIds == null) return;
            foreach (var userId in message.UserIds)
            {
                var user = m_UserRepository.Load(userId);
                var quota = m_UserRepository.GetItemsByUser(user.Id);

                user.Quota.UsedSpace = quota;

                m_UserRepository.Save(user);
            }
        }
    }
}
