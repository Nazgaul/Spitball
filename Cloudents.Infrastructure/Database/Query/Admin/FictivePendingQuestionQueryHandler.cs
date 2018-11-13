using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query.Admin;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Enum;

namespace Cloudents.Infrastructure.Database.Query.Admin
{
    public class FictivePendingQuestionQueryHandler : IQueryHandler<AdminEmptyQuery, IList<FictivePendingQuestionDto>>
    {
        private readonly ISession _session;

        public FictivePendingQuestionQueryHandler(ReadonlySession session)
        {
            _session = session.Session;
        }
        public async Task<IList<FictivePendingQuestionDto>> GetAsync(AdminEmptyQuery query, CancellationToken token)
        {
            var counties = new[] { "us", "il" };

            //var z2222 = _session.Query<Question>()
            //    .Fetch(f => f.User)
            //    .Where(w => w.User.Fictive && w.User.Country == "us")
            //    .OrderBy(o => o.Id)
            //    .Take(1)
            //    .Select(s => s.Id);


            var list = new List<IFutureValue<long>>();
            foreach (var county in counties)
            {
                var future = _session.Query<Question>()
                      .Fetch(f => f.User)
                      .Where(w => w.User.Fictive == true && w.User.Country == county && w.State == QuestionState.Pending)
                      .OrderBy(o => o.Id)
                      .Take(1)
                      .Select(s => s.Id)
                      //.Select(s =>  new FictivePendingQuestionDto(s.Id))
                      .ToFutureValue();
                list.Add(future);
            }
            //var list = counties.Select(county => _session.Query<Question>()
            //        .Fetch(f => f.User)
            //        .Where(w => w.User.Fictive && w.User.Country == county)
            //        .OrderBy(o => o.Id)
            //        .Take(1)
            //        .Select(s => s.Id)
            //        //.Select(s =>  new FictivePendingQuestionDto(s.Id))
            //        .ToFuture())
            //    .ToList();

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