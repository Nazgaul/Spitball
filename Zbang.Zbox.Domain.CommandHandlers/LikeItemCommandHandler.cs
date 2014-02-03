using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class LikeItemCommandHandler : ICommandHandler<LikeItemCommand>
    {
        private readonly IItemLikeRepository m_ItemLikeRepository;
        private readonly IUserRepository m_UserRepository;
        private readonly IRepository<Item> m_ItemRepository;

        public LikeItemCommandHandler(IItemLikeRepository itemLikeRepository,
            IUserRepository userRepository,
            IRepository<Item> itemRepository)
        {
            m_ItemLikeRepository = itemLikeRepository;
            m_UserRepository = userRepository;
            m_ItemRepository = itemRepository;
        }
        public void Handle(LikeItemCommand message)
        {
            var itemLike = m_ItemLikeRepository.GetItemLike(message.UserId, message.ItemId);

            if (itemLike == null)
            {
                AddItemLike(message);
            }
            else
            {
                ChangeItemLike(itemLike);
            }

        }

        private void ChangeItemLike(ItemLike itemLike)
        {
            itemLike.ChangeUserLike();
            m_ItemLikeRepository.Save(itemLike);
        }



        private void AddItemLike(LikeItemCommand message)
        {
            var user = m_UserRepository.Get(message.UserId);
            var item = m_ItemRepository.Get(message.ItemId);

            var itemLike = new ItemLike(user, item);
            m_ItemLikeRepository.Save(itemLike);
        }
    }
}
