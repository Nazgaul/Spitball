using System.Linq;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class RemoveTagsFromItemCommandHandler<T> :
        ICommandHandler<RemoveTagsFromItemCommand> where T : ITag
    {
        private readonly IRepository<T> m_ItemRepository;
        public RemoveTagsFromItemCommandHandler(IRepository<T> itemRepository)
        {
            m_ItemRepository = itemRepository;
        }

        public void Handle(RemoveTagsFromItemCommand message)
        {
            if (!message.Tags.Any())
            {
                return;
            }
            var item = m_ItemRepository.Load(message.ItemId);

            foreach (var tagName in message.Tags)
            {
                item.RemoveTag(tagName);
            }
            m_ItemRepository.Save(item);
        }
    }
}
