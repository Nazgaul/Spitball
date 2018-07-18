using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;

namespace Cloudents.Core
{
    [UsedImplicitly]
    public sealed class QueryBus : IQueryBus, IDisposable
    {
        public QueryBus(ILifetimeScope container)
        {
            _container = container;
        }

        private readonly ILifetimeScope _container;

        public async Task<TQueryResult> QueryAsync<TQueryResult>(IQuery<TQueryResult> query, CancellationToken token)
        {
            using (var child = _container.BeginLifetimeScope())
            {
                var handlerType =
                    typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TQueryResult));

                dynamic handler = child.Resolve(handlerType);
                var t = handler.GetAsync((dynamic)query, token);
                return await t;
            }
        }

        public void Dispose()
        {
            _container?.Dispose();
        }
    }



}
