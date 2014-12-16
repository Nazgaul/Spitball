using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class RateItemCommandHandler : ICommandHandler<RateItemCommand>
    {
        private readonly IItemRateRepository m_ItemRateRepository;
        private readonly IRepository<Item> m_ItemRepository;
        private readonly IUserRepository m_UserRepository;
        private readonly IQueueProvider m_QueueProvider;


        public RateItemCommandHandler(
            IItemRateRepository itemRateRepository,
            IRepository<Item> itemRepository,
            IUserRepository userRepository, IQueueProvider queueProvider)
        {
            m_ItemRateRepository = itemRateRepository;
            m_ItemRepository = itemRepository;
            m_UserRepository = userRepository;
            m_QueueProvider = queueProvider;
        }
        public void Handle(RateItemCommand message)
        {
            if (message == null) throw new ArgumentNullException("message");
            var userRate = m_ItemRateRepository.GetRateOfUser(message.UserId, message.ItemId);

            var item = m_ItemRepository.Load(message.ItemId);
            var user = m_UserRepository.Load(message.UserId);


            if (userRate != null)
            {
                userRate.Rate = message.Rate;
                m_ItemRateRepository.Save(userRate, true);

                var avg = CalculateAverage(message.ItemId);

                item.CalculateRate((int) avg);
                m_ItemRepository.Save(item);
                return;
              

            }
            userRate = new ItemRate(user, item, message.Id, message.Rate);
            m_ItemRateRepository.Save(userRate, true);

            var average = CalculateAverage(message.ItemId);

            item.CalculateRate((int)average);

            m_QueueProvider.InsertMessageToTranaction(new ReputationData(item.Uploader.Id));

            m_ItemRepository.Save(item);
            m_UserRepository.Save(item.Uploader);
        }

        private double CalculateAverage(long itemId)
        {
            return m_ItemRateRepository.CalculateItemAverage(itemId);
        }

       
      
    }
}
