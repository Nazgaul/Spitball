using System;
using System.Linq;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class AssignTagsToItemCommandHandler<T> :
        ICommandHandlerAsync<AssignTagsToItemCommand> where T : ITag
    {
        private readonly IRepository<T> m_ItemRepository;
        private readonly IRepository<Tag> m_TagRepository;
        private readonly IJaredPushNotification m_JaredPush;

        public AssignTagsToItemCommandHandler(IRepository<T> itemRepository,
            IRepository<Tag> tagRepository, IJaredPushNotification jaredPush)
        {
            m_ItemRepository = itemRepository;
            m_TagRepository = tagRepository;
            m_JaredPush = jaredPush;
        }

        public async Task HandleAsync(AssignTagsToItemCommand message)
        {
            if (!message.Tags.Any())
            {
                return;
            }
            var item = m_ItemRepository.Load(message.ItemId);

            foreach (var tagName in message.Tags.Distinct(StringComparer.InvariantCultureIgnoreCase))
            {
                var tag = m_TagRepository.Query().FirstOrDefault(f => f.Name == tagName);
                if (tag == null)
                {
                    tag = new Tag(tagName);
                    m_TagRepository.Save(tag);
                }
                await item.AddTagAsync(tag, message.Type, m_JaredPush).ConfigureAwait(true);
            }

            m_ItemRepository.Save(item);
        }
    }
}