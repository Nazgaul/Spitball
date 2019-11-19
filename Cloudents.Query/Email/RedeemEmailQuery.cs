using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Email
{
    public class RedeemEmailQuery : IQuery<RedeemEmailDto>
    {
        public RedeemEmailQuery(Guid transactionId)
        {
            TransactionId = transactionId;
        }

        private Guid TransactionId { get; }

        internal sealed class RedeemEmailQueryHandler :
            IQueryHandler<RedeemEmailQuery, RedeemEmailDto>
        {
            private readonly IStatelessSession _session;

            public RedeemEmailQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }

            public async Task<RedeemEmailDto> GetAsync(RedeemEmailQuery query, CancellationToken token)
            {
                return await _session.Query<CashOutTransaction>().Where(w => w.Id == query.TransactionId)
                      .Fetch(f => f.User)
                      .Select(s => new RedeemEmailDto()
                      {
                          UserId = s.User.Id,
                          Amount = s.Price,
                          Country = s.User.Country
                      }).FirstOrDefaultAsync(token);
            }
        }
    }
}