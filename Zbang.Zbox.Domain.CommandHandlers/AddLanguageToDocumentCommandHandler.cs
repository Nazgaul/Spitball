using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class AddLanguageToItemCommandHandler<T> : ICommandHandler<AddLanguageToItemCommand> 
        where T :ILanguage
    {
        private readonly IRepository<T> m_ItemRepository;

        public AddLanguageToItemCommandHandler(IRepository<T> itemRepository)
        {
            m_ItemRepository = itemRepository;
        }

        public void Handle(AddLanguageToItemCommand message)
        {
            var item = m_ItemRepository.Load(message.ItemId);
            item.Language = message.Language;
            m_ItemRepository.Save(item);
        }
    }
}