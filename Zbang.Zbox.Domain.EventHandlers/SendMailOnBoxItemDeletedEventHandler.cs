using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Infrastructure.EventHandlers;
using Zbang.Zbox.Domain.Events;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.EventHandlers
{
    class SendMailOnBoxItemDeletedEventHandler : IEventHandler<BoxItemDeletedEvent>
    {
        INotifier m_NotifierManager;
        private IRepository<Box> m_BoxRepository;

        public SendMailOnBoxItemDeletedEventHandler(INotifier notifierManager, IRepository<Box> boxRepository)
        {
            m_NotifierManager = notifierManager;
            m_BoxRepository = boxRepository;
        }

        public void Handle(BoxItemDeletedEvent @event)
        {
            Box box = m_BoxRepository.Get(@event.BoxId);            

            CreateMailParams parametes = new CreateMailParams{
                BoxName = box.BoxName,
                BoxItemName = @event.boxItemName
               
            };
            m_NotifierManager.Notify(@event.BoxId, @event.EmailId, NotificationEventType.Delete,CreateMailParams.ItemDelete, parametes);
        }
    }
}
