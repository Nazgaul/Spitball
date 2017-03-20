using System;
using System.Linq;
using Zbang.Zbox.Domain;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;
namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class SetReviewedCommandHandler<T> : ICommandHandler<SetReviewedCommand> where T : IItem
    {
        private readonly IRepository<T> m_ItemRepository;
        public SetReviewedCommandHandler(IRepository<T> itemRepository)
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
