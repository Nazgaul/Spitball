﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Query.Query.Admin;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query.Admin
{
    public class FictivePendingQuestionQueryHandler : IQueryHandler<AdminEmptyQuery, IList<FictivePendingQuestionDto>>
    {
        private readonly IStatelessSession _session;

        public FictivePendingQuestionQueryHandler(QuerySession session)
        {
            _session = session.StatelessSession;
        }
        public async Task<IList<FictivePendingQuestionDto>> GetAsync(AdminEmptyQuery query, CancellationToken token)
        {
            var counties = new[] { "us"/*, "il" */};

            var list = new List<IFutureValue<long>>();
            // ReSharper disable once LoopCanBeConvertedToQuery - nhibernate doesn't response well for this
            foreach (var county in counties)
            {
                var future = _session.Query<Question>()

                      .Fetch(f => f.User)
                      .Where(w => w.User is SystemUser && w.User.Country == county &&
                                  w.State == ItemState.Pending)
                      .OrderBy(o => o.Id)
                      .Take(1)
                      .Select(s => s.Id)
                      .ToFutureValue();
                list.Add(future);
            }

            var retVal = new List<FictivePendingQuestionDto>();

            foreach (var value in list)
            {
                var z = await value.GetValueAsync(token);
                if (z == default)
                {
                    continue;
                }
                retVal.Add(new FictivePendingQuestionDto(z));
            }
            return retVal;
        }
    }
}