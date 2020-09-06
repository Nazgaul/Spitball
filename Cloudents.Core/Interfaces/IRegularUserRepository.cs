using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;

namespace Cloudents.Core.Interfaces
{
    public interface IRegularUserRepository : IRepository<User>
    {
        Task<decimal> UserBalanceAsync(long userId, CancellationToken token);
        Task<User> GetUserByEmailAsync(string userEmail, CancellationToken token);
        Task<IEnumerable<User>> GetExpiredCreditCardsAsync(CancellationToken token);
    }
}