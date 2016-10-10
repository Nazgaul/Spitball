using System;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;
using Zbang.Zbox.Infrastructure.Query;

namespace Zbang.Zbox.Infrastructure.Cache
{
    public interface IWithCache
    {
        Task<TD> QueryAsync<TQ, TD>(Func<TQ, Task<TD>> getItemCallbackAsync, TQ queryParam)
            where TD : class
            where TQ : IQueryCache;

        Task RemoveAsync<TC>(TC command)
            where TC : ICommandCache;

    }
}
