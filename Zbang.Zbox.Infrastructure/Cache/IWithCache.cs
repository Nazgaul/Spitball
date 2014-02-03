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
        /// <typeparam name="Q">The Query</typeparam>
        /// <typeparam name="D">Dto Result</typeparam>
        /// <param name="getItemCallback">The func to excecute dto</param>
        /// <param name="QueryParam">The query param</param>
        /// <returns>query result</returns>
        D Query<Q, D>(Func<Q, D> getItemCallback, Q QueryParam)
            where D : class
            where Q : IQueryCache;


        Task<D> QueryAsync<Q, D>(Func<Q, Task<D>> getItemCallbackAsync, Q QueryParam)
            where D : class
            where Q : IQueryCache;

        /// <summary>
        /// perform command with cache
        /// </summary>
        /// <typeparam name="C">The command</typeparam>
        /// <typeparam name="CR">Result</typeparam>
        /// <param name="InvokeFunction">The func to excecute the command</param>
        /// <param name="command">command result</param>
        /// <returns></returns>
        CR Command<C, CR>(Func<C, CR> InvokeFunction, C command)
            where CR : ICommandResult
            where C : ICommandCache;

        void Command<C>(Action<C> InvokeFunction, C command)
           where C : ICommandCache;
        //void ClearCacheByKey(string key);
    }
}
