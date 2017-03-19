using System;
using System.Linq;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class AssignTagsToItemCommandHandler<T> :
        ICommandHandler<AssignTagsToItemCommand> where T : IItem
    {
        private readonly IRepository<T> m_ItemRepository;
        private readonly IRepository<Tag> m_TagRepository;
        //private readonly IRepository<ItemTag> m_ItemTagRepository;

        public AssignTagsToItemCommandHandler(IRepository<T> itemRepository,
            IRepository<Tag> tagRepository
            //, IRepository<ItemTag> itemTagRepository
            )
        {
            m_ItemRepository = itemRepository;
            m_TagRepository = tagRepository;
            // m_ItemTagRepository = itemTagRepository;
        }

        public void Handle(AssignTagsToItemCommand message)
        {
            if (!message.Tags.Any())
            {
                return;
            }
            var item = m_ItemRepository.Load(message.ItemId);
            
            foreach (var tagName in message.Tags)
            {
                var tag = m_TagRepository.Query().FirstOrDefault(f => f.Name == tagName);
                if (tag == null)
                {
                    tag = new Tag(tagName);
                    m_TagRepository.Save(tag);
                }

                item.AddTag(tag, message.Type);
            }
            m_ItemRepository.Save(item);
        }

        //protected abstract ItemTag AssignItemToTag(Tag tag, IItem item);
    }

    //public class AssignTagsToDocumentCommandHandler : AssignTagsToItemCommandHandler<Item>
    //{
    //    public AssignTagsToDocumentCommandHandler(IRepository<Item> itemRepository, IRepository<Tag> tagRepository, IRepository<ItemTag> itemTagRepository) : base(itemRepository, tagRepository, itemTagRepository)
    //    {
    //    }

    //    protected override ItemTag AssignItemToTag(Tag tag, IItem item)
    //    {
    //        var document = item as Item;
    //        return new ItemTag(tag, document);
    //    }
    //}
    //public class AssignTagsToFlashcardCommandHandler : AssignTagsToItemCommandHandler<FlashcardMeta>
    //{
    //    public AssignTagsToFlashcardCommandHandler(IRepository<FlashcardMeta> itemRepository, IRepository<Tag> tagRepository, IRepository<ItemTag> itemTagRepository) : base(itemRepository, tagRepository, itemTagRepository)
    //    {
    //    }

    //    protected override ItemTag AssignItemToTag(Tag tag, IItem item)
    //    {
    //        var flashcard = item as FlashcardMeta;
    //        return new ItemTag(tag, flashcard);
    //    }
    //}

}