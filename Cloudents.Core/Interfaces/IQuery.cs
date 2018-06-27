using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    //public interface IQuery
    //{
    //}

    //public interface IQueryResult
    //{

    //}

    public interface IQueryHandlerAsync<in TQuery, TQueryResult>
        //where TQuery : IQuery
       // where TQueryResult : IQueryResult
    {
        Task<TQueryResult> GetAsync(TQuery query, CancellationToken token);
    }

    public interface IQueryHandlerAsync<TQueryResult>
    {
        Task<TQueryResult> GetAsync(CancellationToken token);
    }

    public interface IQueryBus
    {
        Task<TQueryResult> QueryAsync<TQuery, TQueryResult>(TQuery query, CancellationToken token);
           // where TQuery : IQuery;

        Task<TQueryResult> QueryAsync<TQueryResult>(CancellationToken token);

        //where TQueryResult : IQueryResult;
    }
}