using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IQuery
    {
        
    }

    public interface IQueryResult
    {

    }

    public interface IQueryHandlerAsync<in TQuery, TQueryResult>
        where TQuery : IQuery
        where TQueryResult : IQueryResult
    {
        Task<TQueryResult> ExecuteAsync(TQuery command, CancellationToken token);
    }

    public interface IQueryBus
    {
        Task<TQueryResult> QueryAsync<TQuery, TQueryResult>(TQuery command, CancellationToken token)
            where TQuery : IQuery
            where TQueryResult : IQueryResult;
    }
}