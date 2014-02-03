using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class RateItemCommandHandler : ICommandHandler<RateItemCommand>
    {
        private readonly IItemRateRepository m_ItemRateRepositoy;
        private readonly IRepository<Item> m_ItemRepository;
        private readonly IUserRepository m_UserRepository;

        private readonly int[] ReputaionStar = new int[] { 0, 0, 5, 10, 15 };

        public RateItemCommandHandler(
            IItemRateRepository itemRateRepository,
            IRepository<Item> itemRepository,
            IUserRepository userRepository
            )
        {
            m_ItemRateRepositoy = itemRateRepository;
            m_ItemRepository = itemRepository;
            m_UserRepository = userRepository;
        }
        public void Handle(RateItemCommand message)
        {
            var userRate = m_ItemRateRepositoy.GetRateOfUser(message.UserId, message.ItemId);
            var rateCount = m_ItemRateRepositoy.GetRateCount(message.ItemId);

            var item = m_ItemRepository.Load(message.ItemId);




            var user = m_UserRepository.Load(message.UserId);
            if (userRate != null)
            {
                m_ItemRateRepositoy.Delete(userRate);
                item.Uploader.AddReputation(-ReputaionStar[GetReputaionByPoistion(userRate.Rate)]);
                item.RevertRate(userRate.Rate, rateCount);
                --rateCount;

            }
            userRate = new ItemRate(user, item, message.Id, message.Rate);
            item.CalcalateRate(message.Rate, rateCount);
            item.Uploader.AddReputation(ReputaionStar[GetReputaionByPoistion(message.Rate)]);

            m_ItemRateRepositoy.Save(userRate);
            m_ItemRepository.Save(item);
            m_UserRepository.Save(item.Uploader);
        }

        private int GetReputaionByPoistion(int pos)
        {
            return --pos;
        }
    }
}
