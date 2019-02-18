using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Query.Query.Admin;
using NHibernate;
using NHibernate.Criterion;

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
            var counties = new[] { "us", "il", "in" };

            var list = new List<IFutureValue<long>>();
            // ReSharper disable once LoopCanBeConvertedToQuery - nhibernate doesn't response well for this
            foreach (var county in counties)
            {
                Question questionAlias = null;
                SystemUser userAlias = null;

                var future = _session.QueryOver(() => questionAlias)
                        .JoinAlias(x => x.User, () => userAlias)
                        .Select(s => s.Id)
                        .Where(() => userAlias.Country == county)
                        .And(w=>w.Status.State == ItemState.Pending)
                        .OrderBy(Projections.SqlFunction("random_Order", NHibernateUtil.Guid)).Asc
                        .Take(1)
                        .FutureValue<long>();
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