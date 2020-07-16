﻿using Cloudents.Core.DTOs.Email;
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

            public RedeemEmailQueryHandler(IStatelessSession session)
            {
                _session = session;
            }

            public Task<RedeemEmailDto> GetAsync(RedeemEmailQuery query, CancellationToken token)
            {
                return _session.Query<CashOutTransaction>()
                    .WithOptions(w => w.SetComment(nameof(RedeemEmailQuery)))
                    .Where(w => w.Id == query.TransactionId)
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