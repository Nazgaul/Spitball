using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public interface IItemRateRepository : IRepository<ItemRate>
    {
        int GetRateCount(long itemId);
        ItemRate GetRateOfUser(long userId, long itemId);
    }
}
