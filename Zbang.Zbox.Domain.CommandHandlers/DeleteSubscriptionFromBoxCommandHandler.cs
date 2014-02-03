using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Domain.DataAccess;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class DeleteSubscriptionFromBoxCommandHandler : ICommandHandler<DeleteSubscriptionFromBoxCommand, DeleteSubscriptionFromBoxCommandResult>
    {

        private IRepository<Box> m_BoxRepository;
        private ISubscriberRepository m_SubscribeRepository;        


        public DeleteSubscriptionFromBoxCommandHandler(IRepository<Box> boxRepository, ISubscriberRepository subscribeRepository)
        {
            m_BoxRepository = boxRepository;
            m_SubscribeRepository = subscribeRepository;
        }

        public DeleteSubscriptionFromBoxCommandResult Execute(DeleteSubscriptionFromBoxCommand command)
        {
            
            Box box = m_BoxRepository.Get(command.BoxId);
            Subscriber subscriber = m_SubscribeRepository.GetSubsciberByEmail(command.SubscriberEmail, box);
            

            UserPermissionSettings permission = box.GetUserPermission(command.UserId);

          


            if (subscriber.User.UserId != command.UserId && (permission.HasFlag(UserPermissionSettings.Manager) || permission.HasFlag(UserPermissionSettings.Read) || permission.HasFlag(UserPermissionSettings.ReadWrite)))
                throw new UnauthorizedAccessException("User is not authorize to remove user subscription");

            
            box.RemoveSubscriber(subscriber);
            
            m_BoxRepository.Save(box);
            DeleteSubscriptionFromBoxCommandResult result = new DeleteSubscriptionFromBoxCommandResult();

            return result;
        }
    }
}
