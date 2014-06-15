using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class RateItemCommandHandler : ICommandHandler<RateItemCommand>
    {
        private readonly IItemRateRepository m_ItemRateRepositoy;
        private readonly IRepository<Item> m_ItemRepository;
        private readonly IUserRepository m_UserRepository;
        private readonly IRepository<Reputation> m_ReputationRepository;


        public RateItemCommandHandler(
            IItemRateRepository itemRateRepository,
            IRepository<Item> itemRepository,
            IUserRepository userRepository,
            IRepository<Reputation> reputationRepository
            )
        {
            m_ItemRateRepositoy = itemRateRepository;
            m_ItemRepository = itemRepository;
            m_UserRepository = userRepository;
            m_ReputationRepository = reputationRepository;
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
                m_ReputationRepository.Save(item.Uploader.AddReputation(GetUnReputaionByPoistion(userRate.Rate)));
                item.RevertRate(userRate.Rate, rateCount);
                --rateCount;

            }
            userRate = new ItemRate(user, item, message.Id, message.Rate);
            item.CalcalateRate(message.Rate, rateCount);
            m_ReputationRepository.Save(item.Uploader.AddReputation(GetReputaionByPoistion(message.Rate)));

            m_ItemRateRepositoy.Save(userRate);
            m_ItemRepository.Save(item);
            m_UserRepository.Save(item.Uploader);
        }

        private ReputationAction GetReputaionByPoistion(int rate)
        {
            switch (rate)
            {
                case 3:
                    return ReputationAction.Rate3Stars;
                case 4:
                    return ReputationAction.Rate4Stars;
                case 5:
                    return ReputationAction.Rate5Stars;
                default:
                    return ReputationAction.None;
            }

        }
        private ReputationAction GetUnReputaionByPoistion(int rate)
        {
            switch (rate)
            {
                case 3:
                    return ReputationAction.UnRate3Stars;
                case 4:
                    return ReputationAction.UnRate4Stars;
                case 5:
                    return ReputationAction.UnRate5Stars;
                default:
                    return ReputationAction.None;
            }

        }
    }
}
