using System;
using Zbang.Zbox.Domain.CommandHandlers.Resources;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class CreateItemTabCommandHandler : ICommandHandler<CreateItemTabCommand>
    {
        private readonly IItemTabRepository m_ItemTabRepository;
        private readonly IBoxRepository _boxRepository;

        public CreateItemTabCommandHandler(IItemTabRepository itemTabRepository, IBoxRepository boxRepository)
        {
            m_ItemTabRepository = itemTabRepository;
            _boxRepository = boxRepository;
        }

        public void Handle(CreateItemTabCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (string.IsNullOrEmpty(message.Name))
            {
                throw new NullReferenceException("message.Name");
            }

            var box = _boxRepository.Load(message.BoxId);

            var existsBoxTab = m_ItemTabRepository.GetTabWithTheSameName(message.Name, box.Id);
            if (existsBoxTab != null)
            {
                throw new ArgumentException(CommandHandlerResources.CreateItemTabCommandHandler_Handle_Cannot_have_several_tabs_with_the_same_name);
            }

            var itemTab = new ItemTab(message.TabId, message.Name, box);

            m_ItemTabRepository.Save(itemTab);
        }
    }
}
