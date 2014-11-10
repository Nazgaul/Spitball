using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class MarkMessagesAsReadCommandHandler : ICommandHandler<MarkMessagesAsReadCommand>
    {
        private readonly IRepository<InviteToBox> m_InviteRepository;
        public MarkMessagesAsReadCommandHandler(IRepository<InviteToBox> inviteRepository)
        {
            m_InviteRepository = inviteRepository;
        }
        public void Handle(MarkMessagesAsReadCommand commandMessage)
        {
            if (commandMessage == null) throw new ArgumentNullException("commandMessage");
            var message = m_InviteRepository.Get(commandMessage.MessageId);
            message.UpdateMessageAsRead();
            m_InviteRepository.Save(message);
        }
    }
}
