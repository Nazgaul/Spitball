using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class FlagItemAsBadCommandHandler : ICommandHandler<FlagItemAsBadCommand>
    {
        private readonly IRepository<Item> m_ItemRepository;
        public FlagItemAsBadCommandHandler(IRepository<Item> itemRepository)
        {
            m_ItemRepository = itemRepository;
        }

        public void Handle(FlagItemAsBadCommand message)
        {
            var item = m_ItemRepository.Get(message.ItemId);
            Throw.OnNull(item, "item");

            item.FlagItemAsBad();
            m_ItemRepository.Save(item);
        }
    }
}
