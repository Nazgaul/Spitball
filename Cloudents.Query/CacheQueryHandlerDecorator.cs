using Cloudents.Core.Attributes;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Decorators;

namespace Cloudents.Query
{
    public class TransactionQueryDecorator<TQuery, TQueryResult> :
        IQueryHandler<TQuery, TQueryResult>
        where TQuery : IQuery<TQueryResult>

    {
        private readonly IQueryHandler<TQuery, TQueryResult> _decoratee;
        private readonly ReadDbTransaction _transaction;


        public TransactionQueryDecorator(IQueryHandler<TQuery, TQueryResult> decoratee, ReadDbTransaction transaction)
        {
            _decoratee = decoratee;
            _transaction = transaction;
        }

        public async Task<TQueryResult> GetAsync(TQuery query, CancellationToken token)
        {
            return await _decoratee.GetAsync(query, token);
        }
    }

    public class CacheQueryHandlerDecorator<TQuery, TQueryResult> :
        IQueryHandler<TQuery, TQueryResult>
        where TQuery : IQuery<TQueryResult>
        
    {
        private readonly IQueryHandler<TQuery, TQueryResult> _decoratee;
        private readonly Lazy<ICacheProvider>? _cacheProvider;
        private readonly IDecoratorContext _context;
       // private readonly IDecoratedService

        public CacheQueryHandlerDecorator(IQueryHandler<TQuery, TQueryResult> decoratee,
            Lazy<ICacheProvider> cacheProvider, IDecoratorContext context)
        {
            _decoratee = decoratee;
            _cacheProvider = cacheProvider;
            _context = context;
        }

        public CacheQueryHandlerDecorator(IQueryHandler<TQuery, TQueryResult> decoratee, IDecoratorContext context)
        {
            _decoratee = decoratee;
            _context = context;
            // _cacheProvider = cacheProvider;
        }

        public async Task<TQueryResult> GetAsync(TQuery query, CancellationToken token)
        {
            
            var attr = _context.ImplementationType.GetMethod("GetAsync")?.GetCustomAttribute<CacheAttribute>();
            if (attr == null)
            {
                return await _decoratee.GetAsync(query, token);
            }

            if (_cacheProvider is null)
            {
                return await _decoratee.GetAsync(query, token);
            }

            var cacheKey = $"{query.GetType().Name}_{query.AsDictionary(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToContentString()}";

            var result = _cacheProvider.Value.Get<TQueryResult>(cacheKey, attr.Region);

            if (result != null)
            {
                return result;
            }


            result = await _decoratee.GetAsync(query, token);

            _cacheProvider.Value.Set<TQueryResult>(cacheKey, attr.Region, result, TimeSpan.FromMinutes(attr.Duration), attr.Slide);
            return result;


        }
    }
}