using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class MarkMessagesAsOldCommandHandler : ICommandHandler<MarkMessagesAsOldCommand>
    {
        private readonly IInviteRepository m_InviteRepository;
        public MarkMessagesAsOldCommandHandler(IInviteRepository inviteRepository)
        {
            m_InviteRepository = inviteRepository;
        }


        public void Handle(MarkMessagesAsOldCommand commandMessage)
        {
            if (commandMessage == null) throw new ArgumentNullException("commandMessage");
            var invites = m_InviteRepository.GetUserInvites(commandMessage.UserId);
            foreach (var invite in invites)
            {
                invite.UpdateMessageAsOld();
                m_InviteRepository.Save(invite);
            }
        }
    }
}
