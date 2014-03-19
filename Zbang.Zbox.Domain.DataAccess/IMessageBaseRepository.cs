using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public interface IMessageBaseRepository : IRepository<MessageBase>
    {
        IEnumerable<MessageBase> GetCurrentInvites(long recepientId);
    }
}
