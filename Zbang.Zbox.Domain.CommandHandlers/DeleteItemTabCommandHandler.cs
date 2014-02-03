using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Exceptions;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class DeleteItemTabCommandHandler : ICommandHandler<DeleteItemTabCommand>
    {
        private readonly IItemTabRepository m_ItemTabRepository;
        private readonly IUserRepository m_UserRepository;

        public DeleteItemTabCommandHandler(IItemTabRepository itemTabRepository, IUserRepository userRepository)
        {
            m_ItemTabRepository = itemTabRepository;
            m_UserRepository = userRepository;
        }

        public void Handle(DeleteItemTabCommand message)
        {
            Throw.OnNull(message, "message");
            var userType = m_UserRepository.GetUserToBoxRelationShipType(message.UserId, message.BoxId);

            if (userType == Infrastructure.Enums.UserRelationshipType.None || userType == Infrastructure.Enums.UserRelationshipType.Invite)
            {
                throw new UnauthorizedAccessException("You need to follow the box");
            }

            var itemTab = m_ItemTabRepository.Get(message.TabId);
            Throw.OnNull(itemTab, "itemTab");

            if (itemTab.Box.Id != message.BoxId)
            {
                throw new UnauthorizedAccessException("user is not the owner of boxtab");
            }

            itemTab.DeleteReferenceToItems();

            m_ItemTabRepository.Delete(itemTab);
        }
    }
}
