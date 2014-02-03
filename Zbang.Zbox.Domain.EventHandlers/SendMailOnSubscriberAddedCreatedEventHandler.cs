using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Domain.Events;
using Zbang.Zbox.Infrastructure.EventHandlers;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Domain.DataAccess;

namespace Zbang.Zbox.Domain.EventHandlers
{
    public class SendMailOnSubscriberAddedCreatedEventHandler : IEventHandler<BoxSubscriberAddedEvent>
    {
        INotifier m_NotifierManager;
        private IRepository<Box> m_BoxRepository;
        private IUserRepository m_UserRepository;

        public SendMailOnSubscriberAddedCreatedEventHandler(INotifier notifierManager, IRepository<Box> boxRepository, IUserRepository userRepository)
        {
            m_NotifierManager = notifierManager;
            m_BoxRepository = boxRepository;
            m_UserRepository = userRepository;

        }
        public void Handle(BoxSubscriberAddedEvent @event)
        {
            Box box = m_BoxRepository.Get(@event.BoxId);
            User user = m_UserRepository.GetUserByEmail(@event.EmailId);

            CreateMailParams parameters = new CreateMailParams
            {
                BoxName = box.BoxName,
                UserName = user.Email
                
            };
            m_NotifierManager.Notify(@event.BoxId, @event.EmailId, NotificationEventType.Subscription, CreateMailParams.SubscribeToSharedBox, parameters);
        }
    }
}
