using System.Linq;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class AssignTagsToItemCommandHandler : ICommandHandler<AssignTagsToItemCommand>
    {
        private readonly IRepository<Item> m_ItemRepository;
        private readonly IRepository<Tag> m_TagRepository;
        private readonly IRepository<ItemTag> m_ItemTagRepository;

        public AssignTagsToItemCommandHandler(IRepository<Item> itemRepository, 
            IRepository<Tag> tagRepository, IRepository<ItemTag> itemTagRepository
            )
        {
            m_ItemRepository = itemRepository;
            m_TagRepository = tagRepository;
            m_ItemTagRepository = itemTagRepository;
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
                
                var itemTag = new ItemTag(tag, item);
                if (item.ItemTags.Contains(itemTag))
                {
                    continue;
                }
                m_ItemTagRepository.Save(itemTag);
            }
            item.ShouldMakeDirty = () => false;
            m_ItemRepository.Save(item);
        }
    }
}