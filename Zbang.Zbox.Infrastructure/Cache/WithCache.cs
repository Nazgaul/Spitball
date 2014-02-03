using System;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;
using Zbang.Zbox.Infrastructure.Query;

namespace Zbang.Zbox.Infrastructure.Cache
{
    public class WithCache : IWithCache
    {
        ICache m_Cache;
        public WithCache(ICache cache)
        {
            m_Cache = cache;
        }


        public D Query<Q, D>(Func<Q, D> getItemCallback, Q QueryParam)
            where D : class
            where Q : IQueryCache
        {
            string cacheKey = QueryParam.CacheKey;

            D item = m_Cache.GetFromCache(cacheKey, QueryParam.CacheRegion) as D;
            if (item == null)
            {
                item = getItemCallback(QueryParam);
                m_Cache.AddToCache(cacheKey, item, QueryParam.Expiration, QueryParam.CacheRegion, QueryParam.CacheTags);
            }
            return item;
        }

        public async Task<D> QueryAsync<Q, D>(Func<Q, Task<D>> getItemCallbackAsync, Q QueryParam)
            where D : class
            where Q : IQueryCache
        {
            string cacheKey = QueryParam.CacheKey;

            D item = m_Cache.GetFromCache(cacheKey, QueryParam.CacheRegion) as D;
            if (item == null)
            {
                item = await getItemCallbackAsync(QueryParam);
                m_Cache.AddToCache(cacheKey, item, QueryParam.Expiration, QueryParam.CacheRegion, QueryParam.CacheTags);
            }
            return item;
        }

        public CR Command<C, CR>(Func<C, CR> InvokeFunction, C command)
            where CR : ICommandResult
            where C : ICommandCache
        {

            CR retval = InvokeFunction(command);
            m_Cache.RemoveFromCache(command.CacheRegion, command.CacheTags);
            //foreach (var cacheKey in retval.GetCacheKeys())
            //{
            //    m_Cache.RemoveFromCache(cacheKey);
            //}
            return retval;
        }
        //public void ClearCacheByKey(string key)
        //{
        //    m_Cache.RemoveFromCache(key);
        //}

        public void Command<C>(Action<C> InvokeFunction, C command)
            where C : ICommandCache
        {
            InvokeFunction(command);
            m_Cache.RemoveFromCache(command.CacheRegion, command.CacheTags);
        }
    }

    public static class ConstCacheKeys
    {
        //public const string kItem = "Item";
        //public const string kComment = "Comment";
        //public const string kBoxUsers = "BoxUsers";
        //public const string kUserRights = "UserRights";
        //public const string Box = "box";
        public const string kSeperator = "_";

        public const string Items = "Items";
        public const string Comments = "Comments";

        public const string kShortCodes = "ShortCodes";
    }


}
