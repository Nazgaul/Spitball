using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class PreviewFailedCommandHandler : ICommandHandler<PreviewFailedCommand>
    {
        private readonly IRepository<File> _itemRepository;

        public PreviewFailedCommandHandler(IRepository<File> itemRepository)
        {
            _itemRepository = itemRepository;
        }

        public void Handle(PreviewFailedCommand message)
        {
            var item = _itemRepository.Load(message.ItemId);
            item.PreviewFailed = true;
            _itemRepository.Save(item);
        }
    }
}
