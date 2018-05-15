﻿using System;
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

        public async Task<TQueryResult> QueryAsync<TQuery, TQueryResult>(TQuery query, CancellationToken token) where TQuery : IQuery //where TQueryResult : IQueryResult
        {
            using (var child = _container.BeginLifetimeScope())
            {
                var obj = child.Resolve<IQueryHandlerAsync<TQuery, TQueryResult>>();
                return await obj.GetAsync(query, token).ConfigureAwait(false);
            }
        }

        public async Task<TQueryResult> QueryAsync<TQueryResult>(CancellationToken token)
        {
            //IQueryHandlerAsync<TQueryResult>
            using (var child = _container.BeginLifetimeScope())
            {
                var obj = child.Resolve<IQueryHandlerAsync<TQueryResult>>();
                return await obj.GetAsync(token).ConfigureAwait(false);
            }
        }

        public void Dispose()
        {
            _container?.Dispose();
        }
    }
}
