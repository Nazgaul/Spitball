using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class AssignItemToTabCommandHandler : ICommandHandler<AssignItemToTabCommand>
    {
        private readonly IItemTabRepository m_ItemTabRepository;
        private readonly IRepository<Item> m_ItemRepository;

        public AssignItemToTabCommandHandler(IItemTabRepository itemTabRepository,
           IRepository<Item> itemRepository)
        {
            m_ItemTabRepository = itemTabRepository;
            m_ItemRepository = itemRepository;
        }

        public void Handle(AssignItemToTabCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            var item = m_ItemRepository.Load(message.ItemId);
            ItemTab itemTab;
            item.ShouldMakeDirty = () => true;
            if (!message.TabId.HasValue)
            {
                itemTab = item.Tab;
                itemTab.DeleteItemFromTab(item);
            }
            else
            {
                itemTab = m_ItemTabRepository.Load(message.TabId.Value);
                itemTab.AddItemToTab(item);
            }
            m_ItemRepository.Save(item);
            m_ItemTabRepository.Save(itemTab);
        }
    }
}
