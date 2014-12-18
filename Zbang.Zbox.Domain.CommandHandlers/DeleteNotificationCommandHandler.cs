using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class DeleteNotificationCommandHandler : ICommandHandler<DeleteNotificationCommand>
    {
        private readonly IRepository<Invite> m_InviteRepository;
        public DeleteNotificationCommandHandler(IRepository<Invite> inviteRepository)
        {
            m_InviteRepository = inviteRepository;
        }
        public void Handle(DeleteNotificationCommand commandMessage)
        {
            if (commandMessage == null) throw new ArgumentNullException("commandMessage");
            var message = m_InviteRepository.Load(commandMessage.MessageId);
            m_InviteRepository.Delete(message);
        }
    }
}
