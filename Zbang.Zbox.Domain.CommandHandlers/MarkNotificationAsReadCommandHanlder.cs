using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class MarkNotificationAsReadCommandHanlder : ICommandHandler<MarkNotificationAsReadCommand>
    {
        private readonly IMessageBaseRepository m_MessageRepositoy;
        public MarkNotificationAsReadCommandHanlder(IMessageBaseRepository messageRepository)
        {
            m_MessageRepositoy = messageRepository;
        }
        public void Handle(MarkNotificationAsReadCommand commandMessage)
        {
            var messages = m_MessageRepositoy.GetCurrentInvites(commandMessage.UserId);
            foreach (var message in messages)
            {
                message.UpdateMessageAsRead();
                m_MessageRepositoy.Save(message);
            }
        }
    }
}
