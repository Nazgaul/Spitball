using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class PreviewFailedCommandHandler : ICommandHandler<PreviewFailedCommand>
    {
        private readonly IRepository<File> m_ItemRepository;

        public PreviewFailedCommandHandler(IRepository<File> itemRepository)
        {
            m_ItemRepository = itemRepository;
        }

        public void Handle(PreviewFailedCommand message)
        {
            var item = m_ItemRepository.Load(message.ItemId);
            item.PreviewFailed = true;
            m_ItemRepository.Save(item);
        }
    }

    
}
