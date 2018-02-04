using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;
namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class SetReviewedCommandHandler : ICommandHandler<SetReviewedCommand>
    {
        private readonly IRepository<Item> m_ItemRepository;
        public SetReviewedCommandHandler(IRepository<Item> itemRepository)
        {
            m_ItemRepository = itemRepository;
        }

        public void Handle(SetReviewedCommand message)
        {
            var item = m_ItemRepository.Load(message.ItemId);
            item.IsReviewed = true;
            m_ItemRepository.Save(item);
        }
    }
}
