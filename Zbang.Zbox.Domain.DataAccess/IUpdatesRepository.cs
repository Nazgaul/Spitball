using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public interface IUpdatesRepository: IRepository<Updates>
    {
        IEnumerable<Updates> GetUserBoxUpdates(long userId, long boxId);
    }
}
