using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class AddLanguageToItemCommandHandler<T> : ICommandHandler<AddLanguageToItemCommand> 
        where T :IItem
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
            item.ShouldMakeDirty = () => false;
            m_ItemRepository.Save(item);
        }
    }
    //public class AddLanguageToDocumentCommandHandler : AddLanguageToItemCommandHandler
    //{
    //    //private readonly IRepository<Item> m_ItemRepository;

    //    public AddLanguageToDocumentCommandHandler(IRepository<Item> itemRepository) 
    //        : base(itemRepository)
    //    {
    //        //m_ItemRepository = itemRepository;
    //    }

    //    public void Handle(AddLanguageToDocumentCommand message)
    //    {
    //        var item = m_ItemRepository.Load(message.ItemId);
    //        item.Language = message.Language;
    //        item.ShouldMakeDirty = () => false;
    //        m_ItemRepository.Save(item);
    //    }
    //}

}