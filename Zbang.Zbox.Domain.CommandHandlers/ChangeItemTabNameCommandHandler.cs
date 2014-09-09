using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class ChangeItemTabNameCommandHandler : ICommandHandler<ChangeItemTabNameCommand>
    {
        private readonly IItemTabRepository m_ItemTabRepository;
        private readonly IUserRepository m_UserRepository;

        public ChangeItemTabNameCommandHandler(IItemTabRepository itemTabRepository,
            IUserRepository userRepository)
        {
            m_ItemTabRepository = itemTabRepository;
            m_UserRepository = userRepository;
        }
        public void Handle(ChangeItemTabNameCommand message)
        {
            if (message == null) throw new ArgumentNullException("message");
            var userType = m_UserRepository.GetUserToBoxRelationShipType(message.UserId, message.BoxId);

            if (userType == Infrastructure.Enums.UserRelationshipType.None || userType == Infrastructure.Enums.UserRelationshipType.Invite)
            {
                throw new UnauthorizedAccessException("You need to follow the box");
            }

            var itemTab = m_ItemTabRepository.Get(message.TabId);

            if (itemTab == null)
            {
                throw new NullReferenceException("itemTab");
            }


            if (itemTab.Box.Id != message.BoxId)
            {
                throw new UnauthorizedAccessException("user is not connected to box");
            }

            var boxTabWithSameName = m_ItemTabRepository.GetTabWithTheSameName(message.NewName, message.UserId);
            if (boxTabWithSameName != null)
            {
                throw new ArgumentException("You cannot change to name to already same name");
            }

            itemTab.ChangeName(message.NewName);
            m_ItemTabRepository.Save(itemTab);


        }

    }
}
