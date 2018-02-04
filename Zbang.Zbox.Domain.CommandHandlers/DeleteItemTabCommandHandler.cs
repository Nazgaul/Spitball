using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class DeleteItemTabCommandHandler : ICommandHandler<DeleteItemTabCommand>
    {
        private readonly IItemTabRepository m_ItemTabRepository;
        private readonly IUserRepository _userRepository;

        public DeleteItemTabCommandHandler(IItemTabRepository itemTabRepository, IUserRepository userRepository)
        {
            m_ItemTabRepository = itemTabRepository;
            _userRepository = userRepository;
        }

        public void Handle(DeleteItemTabCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var userType = _userRepository.GetUserToBoxRelationShipType(message.UserId, message.BoxId);

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
                throw new UnauthorizedAccessException("user is not the owner of boxtab");
            }

            itemTab.DeleteReferenceToItems();

            m_ItemTabRepository.Delete(itemTab);
        }
    }
}
