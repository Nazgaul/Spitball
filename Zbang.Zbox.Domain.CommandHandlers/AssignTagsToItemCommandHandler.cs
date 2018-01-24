using System;
using System.Linq;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class AssignTagsToItemCommandHandler<T> :
        ICommandHandlerAsync<AssignTagsToItemCommand> where T : ITag
    {
        private readonly IRepository<T> _itemRepository;
        private readonly IRepository<Tag> _tagRepository;

        public AssignTagsToItemCommandHandler(IRepository<T> itemRepository,
            IRepository<Tag> tagRepository)
        {
            _itemRepository = itemRepository;
            _tagRepository = tagRepository;
        }

        public async Task HandleAsync(AssignTagsToItemCommand message)
        {
            if (!message.Tags.Any())
            {
                return;
            }
            var item = _itemRepository.Load(message.ItemId);

            foreach (var tagName in message.Tags.Distinct(StringComparer.InvariantCultureIgnoreCase))
            {
                var tag = _tagRepository.Query().FirstOrDefault(f => f.Name == tagName);
                if (tag == null)
                {
                    tag = new Tag(tagName);
                    _tagRepository.Save(tag);
                }
                await item.AddTagAsync(tag, message.Type).ConfigureAwait(true);
            }

            _itemRepository.Save(item);
        }
    }
}