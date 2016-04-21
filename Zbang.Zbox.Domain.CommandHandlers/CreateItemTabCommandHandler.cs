﻿using System;
using Zbang.Zbox.Domain.CommandHandlers.Resources;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class CreateItemTabCommandHandler : ICommandHandler<CreateItemTabCommand>
    {
        private readonly IItemTabRepository m_ItemTabRepository;
        private readonly IBoxRepository m_BoxRepository;
        private readonly IUserRepository m_UserRepository;

        public CreateItemTabCommandHandler(IItemTabRepository itemTabRepository, IBoxRepository boxRepository,
            IUserRepository userRepository)
        {
            m_ItemTabRepository = itemTabRepository;
            m_BoxRepository = boxRepository;
            m_UserRepository = userRepository;
        }
        public void Handle(CreateItemTabCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (string.IsNullOrEmpty(message.Name))
            {
                throw new NullReferenceException("message.Name");
            }

            var box = m_BoxRepository.Load(message.BoxId);

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
