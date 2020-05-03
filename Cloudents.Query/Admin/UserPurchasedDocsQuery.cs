using Cloudents.Core.DTOs.Admin;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query.Admin
{
    public class UserPurchasedDocsQuery : IQueryAdmin2<IEnumerable<UserPurchasedDocsDto>>
    {
        public UserPurchasedDocsQuery(long userId, int page, Country? country)
        {
            UserId = userId;
            Page = page;
            Country = country;
        }
        private long UserId { get; }
        private int Page { get; }
        public Country? Country { get; }
        internal sealed class UserPurchasedDocsQueryHandler : IQueryHandler<UserPurchasedDocsQuery, IEnumerable<UserPurchasedDocsDto>>
        {
            private readonly IStatelessSession _session;


            public UserPurchasedDocsQueryHandler(QuerySession dapper)
            {
                _session = dapper.StatelessSession;
            }
            private const int PageSize = 200;


            public async Task<IEnumerable<UserPurchasedDocsDto>> GetAsync(UserPurchasedDocsQuery query, CancellationToken token)
            {

                var dbQuery = _session.Query<DocumentTransaction>()
                    .Fetch(f=>f.Document)
                    .Where(w => w.User.Id == query.UserId && w.Type == TransactionType.Spent);
                if (query.Country != null)
                {
                    dbQuery = dbQuery.Where(w => w.User.SbCountry == query.Country);
                        //(_session.Query<User>().Where(w2=>w2.SbCountry == query.Country)).Select(s=>s.Id).Contains(w.User.Id));
                }

                return await dbQuery.OrderBy(o => o.Document.Id)
                    .Take(PageSize).Skip(PageSize * query.Page)
                    .Select(s => new UserPurchasedDocsDto()
                    {
                        Price = s.Price,
                        Class = s.Document.Course.Id,
                        DocumentId = s.Document.Id,
                        Title = s.Document.Name,

                    }).ToListAsync(token);
                
            }
        }
    }
}
