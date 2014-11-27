using System;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;
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

        /// <summary>
        /// perform command with cache
        /// </summary>
        /// <typeparam name="TC">The command</typeparam>
        /// <typeparam name="TCr">Result</typeparam>
        /// <param name="invokeFunction">The func to excecute the command</param>
        /// <param name="command">command result</param>
        /// <returns></returns>
        //TCr Command<TC, TCr>(Func<TC, TCr> invokeFunction, TC command)
        //    where TCr : ICommandResult
        //    where TC : ICommandCache;

        //void Command<TC>(Action<TC> invokeFunction, TC command)
        //   where TC : ICommandCache;
        //void ClearCacheByKey(string key);
    }
}
