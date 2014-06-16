using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public interface IMessageBaseRepository : IRepository<MessageBase>
    {
        IEnumerable<MessageBase> GetCurrentInvites(long recepientId);
    }
}
