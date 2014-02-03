using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public interface IItemLikeRepository : IRepository<ItemLike>
    {
        ItemLike GetItemLike(long userId, long itemId);
    }
}
