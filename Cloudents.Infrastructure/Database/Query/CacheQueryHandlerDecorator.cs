using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Attributes;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Interceptor;

namespace Cloudents.Infrastructure.Database.Query
{
    public class CacheQueryHandlerDecorator<TQuery, TQueryResult> : IQueryHandler<TQuery, TQueryResult> where TQuery : IQuery<TQueryResult>
    {
        private readonly IQueryHandler<TQuery, TQueryResult> _decoratee;
        private readonly ICacheProvider _cacheProvider;

        public CacheQueryHandlerDecorator(IQueryHandler<TQuery, TQueryResult> decoratee, ICacheProvider cacheProvider)
        {
            _decoratee = decoratee;
            _cacheProvider = cacheProvider;
        }

        public async Task<TQueryResult> GetAsync(TQuery query, CancellationToken token)
        {
            var attr = _decoratee.GetType().GetMethod("GetAsync")?.GetCustomAttribute<CacheAttribute>();
            if (attr == null)
            {
                return await _decoratee.GetAsync(query, token);
            }

            var cacheKey = CacheResultInterceptor.GetCacheKey(_decoratee.GetType(), "GetAsync", new object[] { query });

            var result = _cacheProvider.Get<TQueryResult>(cacheKey, attr.Region);

            if (result != null)
            {
                return result;
            }
            

            result = await _decoratee.GetAsync(query, token);

            _cacheProvider.Set(cacheKey,attr.Region,result,attr.Duration,attr.Slide);
            return result;


        }
    }
}