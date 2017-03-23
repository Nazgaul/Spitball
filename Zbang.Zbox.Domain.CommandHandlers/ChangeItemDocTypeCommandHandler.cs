using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    class ChangeItemDocTypeCommandHandler: ICommandHandler<ChangeItemDocTypeCommand>
    {
        private readonly IRepository<Item> m_ItemRepository;
        public ChangeItemDocTypeCommandHandler(IRepository<Item> itemRepository)
        {
            m_ItemRepository = itemRepository;
        }

        public void Handle(ChangeItemDocTypeCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var item = m_ItemRepository.Load(message.ItemId);
            item.DocType = message.DocType;
            item.ShouldMakeDirty = () => true;
            m_ItemRepository.Save(item);
        }
    }
}
