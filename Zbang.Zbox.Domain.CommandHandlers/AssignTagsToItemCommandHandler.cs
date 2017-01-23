using System.Linq;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class AssignTagsToItemCommandHandler : ICommandHandler<AssignTagsToItemCommand>
    {
        private readonly IRepository<Item> m_ItemRepository;
        private readonly IRepository<Tag> m_TagRepository;
        private readonly IRepository<ItemTag> m_ItemTagRepository;
        private readonly IRepository<CourseTag> m_CourseTagRepository;
        private readonly IGuidIdGenerator m_GuidIdGenerator;

        public AssignTagsToItemCommandHandler(IRepository<Item> itemRepository, IRepository<Tag> tagRepository, IRepository<ItemTag> itemTagRepository, IRepository<CourseTag> courseTagRepository, IGuidIdGenerator guidIdGenerator)
        {
            m_ItemRepository = itemRepository;
            m_TagRepository = tagRepository;
            m_ItemTagRepository = itemTagRepository;
            m_CourseTagRepository = courseTagRepository;
            m_GuidIdGenerator = guidIdGenerator;
        }

        public void Handle(AssignTagsToItemCommand message)
        {
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
            m_ItemRepository.Save(item);
        }
    }

    public class AddLanguageToDocumentCommandHandler : ICommandHandler<AddLanguageToDocumentCommand>
    {
        private readonly IRepository<Item> m_ItemRepository;

        public AddLanguageToDocumentCommandHandler(IRepository<Item> itemRepository)
        {
            m_ItemRepository = itemRepository;
        }

        public void Handle(AddLanguageToDocumentCommand message)
        {
            var item = m_ItemRepository.Load(message.ItemId);
            item.Language = message.Language;
            m_ItemRepository.Save(item);
        }
    }
}