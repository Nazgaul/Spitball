using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query
{
    // ReSharper disable once UnusedTypeParameter - using in dynamic reflection
    public interface IQuery<TResult>
    {
    }


    public interface IQueryHandler<in TQuery, TQueryResult>
       where TQuery : IQuery<TQueryResult>
    {
        Task<TQueryResult> GetAsync(TQuery query, CancellationToken token);
    }


    public interface IQueryBus
    {
        Task<TQueryResult> QueryAsync<TQueryResult>(IQuery<TQueryResult> query, CancellationToken token);
    }
}