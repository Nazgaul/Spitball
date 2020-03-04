using Autofac;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query
{
    public sealed class QueryBus : IQueryBus//, IDisposable
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
                try
                {
                    var handlerType =
                        typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TQueryResult));

                    dynamic handler = child.Resolve(handlerType);
                    var t = handler.GetAsync((dynamic)query, token);
                    return await t;
                }
                catch (InvalidOperationException e) //db return this type of exception and not cancellation token
                {
                    if (token.IsCancellationRequested)
                    {
                        throw new OperationCanceledException("on query", e);
                    }

                    if (e.Message.Contains("Operation cancelled by user."))
                    {
                        throw new OperationCanceledException("on query", e);
                    }

                    throw;
                }
            }
        }

        //public void Dispose()
        //{
        //    _container?.Dispose();
        //}
    }



}
