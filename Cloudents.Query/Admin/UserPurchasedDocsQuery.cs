using Cloudents.Core.DTOs.Admin;
using Dapper;
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
        public UserPurchasedDocsQuery(long userId, int page, Country country)
        {
            UserId = userId;
            Page = page;
            Country = country;
        }
        private long UserId { get; }
        private int Page { get; }
        public Country Country { get; }
        internal sealed class UserPurchasedDocsQueryHandler : IQueryHandler<UserPurchasedDocsQuery, IEnumerable<UserPurchasedDocsDto>>
        {
            private readonly IStatelessSession _statelessSession;


            public UserPurchasedDocsQueryHandler(QuerySession dapper)
            {
                _statelessSession = dapper.StatelessSession;
            }
            private const int PageSize = 200;


            public async Task<IEnumerable<UserPurchasedDocsDto>> GetAsync(UserPurchasedDocsQuery query, CancellationToken token)
            {

                var x = _statelessSession.Query<DocumentTransaction>()
                    .Fetch(f => f.Document)
                    .ThenFetch(f => f.Course2)
                    .Where(w => w.Type == TransactionType.Spent);

                if (query.Country != null)
                {
                    x = x.Where(w => w.User.SbCountry == query.Country);
                }
                return await x.OrderBy(o => o.Document.Id)
                    .Take(PageSize)
                    .Skip(PageSize * query.Page)
                    .Select(s => new UserPurchasedDocsDto()
                    {
                        Class = s.Document.Course2.SearchDisplay,
                        DocumentId = s.Document.Id,
                        Price = s.Price,
                        Title = s.Document.Name
                    }).ToListAsync(token);

                //const string sql = @"select DocumentId, D.Name as Title,
                // D.CourseName as Class, T.Price
                //    from sb.[Transaction] T
                //    join sb.Document D
	               //     on T.DocumentId = D.Id
                 
                //    where User_Id = @Id and TransactionType = 'Document' and T.[Type] = 'Spent'
                //    and User_Id in (select Id from sb.[user] where Id = User_Id and (Country = @Country or @Country is null))
                //    order by 1
                //    OFFSET @pageSize * @PageNumber ROWS
                //    FETCH NEXT @pageSize ROWS ONLY;";
                //using var connection = _dapper.OpenConnection();
                //return await connection.QueryAsync<UserPurchasedDocsDto>(sql,
                //    new
                //    {
                //        id = query.UserId,
                //        PageNumber = query.Page,
                //        PageSize,
                //        query.Country
                //    });
            }
        }
    }
}
