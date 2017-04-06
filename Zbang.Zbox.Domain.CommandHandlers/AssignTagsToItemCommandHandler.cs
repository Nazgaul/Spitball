using System.Linq;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class AssignTagsToItemCommandHandler<T> :
        ICommandHandler<AssignTagsToItemCommand> where T : ITag
    {
        private readonly IRepository<T> m_ItemRepository;
        private readonly IRepository<Tag> m_TagRepository;

        public AssignTagsToItemCommandHandler(IRepository<T> itemRepository,
            IRepository<Tag> tagRepository
            )
        {
            m_ItemRepository = itemRepository;
            m_TagRepository = tagRepository;
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
    }
}