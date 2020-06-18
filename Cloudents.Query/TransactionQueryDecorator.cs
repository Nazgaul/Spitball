using System.Threading;
using System.Threading.Tasks;

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
            var x =  await _decoratee.GetAsync(query, token);

            _transaction.FinishTransaction();
            return x;
        }
    }
}