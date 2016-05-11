using System;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;
using Zbang.Zbox.Infrastructure.Query;

namespace Zbang.Zbox.Infrastructure.Cache
{
    public interface IWithCache
    {
        //TDtoResult Query<TQueryCache, TDtoResult>(Func<TQueryCache, TDtoResult> getItemCallback, TQueryCache queryParam)
        //    where TDtoResult : class
        //    where TQueryCache : IQueryCache;


        Task<TD> QueryAsync<TQ, TD>(Func<TQ, Task<TD>> getItemCallbackAsync, TQ queryParam)
            where TD : class
            where TQ : IQueryCache;

        //Task<TD> QueryAsync<TQ, TD>(Func<TQ, CancellationToken, Task<TD>> getItemCallbackAsync, TQ queryParam,
        //    CancellationToken token)
        //    where TD : class
        //    where TQ : IQueryCache;

        Task CommandAsync<TC>(TC command)
            where TC : ICommandCache;

    }
}
