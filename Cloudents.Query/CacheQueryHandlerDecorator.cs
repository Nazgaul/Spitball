using Cloudents.Core.Attributes;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query
{
    public class CacheQueryHandlerDecorator<TQuery, TQueryResult> :
        IQueryHandler<TQuery, TQueryResult> where TQuery : IQuery<TQueryResult>
    {
        private readonly IQueryHandler<TQuery, TQueryResult> _decoratee;
        private readonly Lazy<ICacheProvider> _cacheProvider;

        public CacheQueryHandlerDecorator(IQueryHandler<TQuery, TQueryResult> decoratee, Lazy<ICacheProvider> cacheProvider)
        {
            _decoratee = decoratee;
            _cacheProvider = cacheProvider;
        }

        public CacheQueryHandlerDecorator(IQueryHandler<TQuery, TQueryResult> decoratee)
        {
            _decoratee = decoratee;
            // _cacheProvider = cacheProvider;
        }

        public async Task<TQueryResult> GetAsync(TQuery query, CancellationToken token)
        {
            var attr = _decoratee.GetType().GetMethod("GetAsync")?.GetCustomAttribute<CacheAttribute>();
            if (attr == null)
            {
                return await _decoratee.GetAsync(query, token);
            }

            if (_cacheProvider is null)
            {
                return await _decoratee.GetAsync(query, token);
            }

            var cacheKey = $"{query.GetType().Name}_{query.AsDictionary(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToContentString()}";
            //var cacheKey = CacheResultInterceptor.GetCacheKey(_decoratee.GetType(), "GetAsync", new object[] { query });

            var result = _cacheProvider.Value.Get<TQueryResult>(cacheKey, attr.Region);

            if (result != null)
            {
                return result;
            }


            result = await _decoratee.GetAsync(query, token);

            _cacheProvider.Value.Set(cacheKey, attr.Region, result, attr.Duration, attr.Slide);
            return result;


        }
    }
}