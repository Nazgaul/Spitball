using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class MarkMessagesAsOldCommandHanlder : ICommandHandler<MarkMessagesAsOldCommand>
    {
        private readonly IMessageBaseRepository m_MessageRepositoy;
        public MarkMessagesAsOldCommandHanlder(IMessageBaseRepository messageRepository)
        {
            m_MessageRepositoy = messageRepository;
        }


        public void Handle(MarkMessagesAsOldCommand commandMessage)
        {
            var messages = m_MessageRepositoy.GetCurrentInvites(commandMessage.UserId);
            foreach (var message in messages)
            {
                message.UpdateMessageAsOld();
                m_MessageRepositoy.Save(message);
            }
        }
    }
}
