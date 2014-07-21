using System;
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
            if (message == null) throw new ArgumentNullException("message");
            if (string.IsNullOrEmpty(message.Name))
            {
                throw new NullReferenceException("message.Name");
            }


            var userType = m_UserRepository.GetUserToBoxRelationShipType(message.UserId, message.BoxId);
            if (userType == Infrastructure.Enums.UserRelationshipType.None || userType == Infrastructure.Enums.UserRelationshipType.Invite)
            {
                throw new UnauthorizedAccessException("You need to follow the box");
            }


            var box = m_BoxRepository.Get(message.BoxId);
            if (box == null)
            {
                throw new NullReferenceException("box");
            }

            var existsBoxTab = m_ItemTabRepository.GetTabWithTheSameName(message.Name, box.Id);
            if (existsBoxTab != null)
            {
                throw new ArgumentException("Cannot have several tabs with the same name");
            }

            var itemTab = new ItemTab(message.TabId, message.Name, box);

            m_ItemTabRepository.Save(itemTab);
        }
    }
}
