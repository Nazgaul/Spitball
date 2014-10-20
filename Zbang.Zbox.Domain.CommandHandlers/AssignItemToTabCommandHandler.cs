using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class AssignItemToTabCommandHandler : ICommandHandler<AssignItemToTabCommand>
    {
        private readonly IUserRepository m_UserRepository;
        private readonly IItemTabRepository m_ItemTabRepository;
        private readonly IRepository<Item> m_ItemRepository;

        public AssignItemToTabCommandHandler(IUserRepository userRepository, IItemTabRepository itemTabRepository,
           IRepository<Item> itemRepository)
        {
            m_UserRepository = userRepository;
            m_ItemTabRepository = itemTabRepository;
            m_ItemRepository = itemRepository;

        }

        public void Handle(AssignItemToTabCommand message)
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
            if (message.NeedDelete)
            {
                itemTab.DeleteReferenceToItems();
            }
            foreach (var itemid in message.ItemsId)
            {
                var item = m_ItemRepository.Get(itemid);
                if (item == null)
                {
                    throw new NullReferenceException("item");
                }


                itemTab.AddItemToTab(item);

                m_ItemTabRepository.Save(itemTab);
            }
        }
    }
}
