using System;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Query;

namespace Zbang.Zbox.Infrastructure.Cache
{
    public interface IWithCache
    {
        /// <summary>
        /// perform dto to query with cache
        /// </summary>
        /// <typeparam name="TQ">The Query</typeparam>
        /// <typeparam name="TD">Dto Result</typeparam>
        /// <param name="getItemCallback">The func to excecute dto</param>
        /// <param name="queryParam">The query param</param>
        /// <returns>query result</returns>
        TD Query<TQ, TD>(Func<TQ, TD> getItemCallback, TQ queryParam)
            where TD : class
            where TQ : IQueryCache;


        Task<TD> QueryAsync<TQ, TD>(Func<TQ, Task<TD>> getItemCallbackAsync, TQ queryParam)
            where TD : class
            where TQ : IQueryCache;

        Task<TD> QueryAsync<TQ, TD>(Func<TQ, CancellationToken, Task<TD>> getItemCallbackAsync, TQ queryParam,
            CancellationToken token)
            where TD : class
            where TQ : IQueryCache;

    }
}
