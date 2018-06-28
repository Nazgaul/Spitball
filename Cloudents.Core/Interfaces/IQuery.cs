using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IQuery<TResult>
    {
    }

    //public interface IQueryResult
    //{

    //}

    //public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    //{
    //    Task<TResult> HandleAsync(TQuery query);
    //}

    //public interface IQueryProcessor
    //{
    //    Task<TResult> ProcessAsync<TResult>(IQuery<TResult> query);
    //}

    public interface IQueryHandlerAsync<in TQuery, TQueryResult>
       where TQuery : IQuery<TQueryResult>
    // where TQueryResult : IQueryResult
    {
        Task<TQueryResult> GetAsync(TQuery query, CancellationToken token);
    }

    //public interface IQueryHandlerAsync<TQueryResult>
    //{
    //    Task<TQueryResult> GetAsync(CancellationToken token);
    //}

    public interface IQueryBus
    {
        Task<TQueryResult> QueryAsync<TQueryResult>(IQuery<TQueryResult> query, CancellationToken token);
        //where TQuery : IQuery<TQueryResult>;

        //Task<TQueryResult> QueryAsync<TQueryResult>(CancellationToken token);

        //where TQueryResult : IQueryResult;
    }
}