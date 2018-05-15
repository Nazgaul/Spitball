using System;
using System.Collections.Generic;
using System.Text;
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

        public async Task<TQueryResult> QueryAsync<TQuery, TQueryResult>(TQuery command, CancellationToken token) where TQuery : IQuery where TQueryResult : IQueryResult
        {
            using (var child = _container.BeginLifetimeScope())
            {
                var obj = child.Resolve<IQueryHandlerAsync<TQuery, TQueryResult>>();
                return await obj.ExecuteAsync(command, token).ConfigureAwait(false);
            }
        }

        public void Dispose()
        {
            _container?.Dispose();
        }
    }
}
