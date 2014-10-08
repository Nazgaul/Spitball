using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class MarkMessagesAsReadCommandHanlder : ICommandHandler<MarkMessagesAsReadCommand>
    {
        private readonly IMessageBaseRepository m_MessageRepositoy;
        public MarkMessagesAsReadCommandHanlder(IMessageBaseRepository messageRepository)
        {
            m_MessageRepositoy = messageRepository;
        }
        public void Handle(MarkMessagesAsReadCommand commandMessage)
        {
            if (commandMessage == null) throw new ArgumentNullException("commandMessage");
            var message =  m_MessageRepositoy.Load(commandMessage.MessageId);
            if (message.Recipient.Id != commandMessage.UserId)
            {
                throw new ArgumentException("User is not the recepients");
            }
            message.UpdateMessageAsRead();
            m_MessageRepositoy.Save(message);
        }
    }
}
