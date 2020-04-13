using Cloudents.Core.DTOs.Admin;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Admin
{
    public class UserPurchasedDocsQuery : IQueryAdmin<IEnumerable<UserPurchasedDocsDto>>
    {
        public UserPurchasedDocsQuery(long userId, int page, string country)
        {
            UserId = userId;
            Page = page;
            Country = country;
        }
        private long UserId { get; }
        private int Page { get; }
        public string Country { get; }
        internal sealed class UserPurchasedDocsQueryHandler : IQueryHandler<UserPurchasedDocsQuery, IEnumerable<UserPurchasedDocsDto>>
        {
            private readonly IDapperRepository _dapper;


            public UserPurchasedDocsQueryHandler(IDapperRepository dapper)
            {
                _dapper = dapper;
            }
            private const int PageSize = 200;


            public async Task<IEnumerable<UserPurchasedDocsDto>> GetAsync(UserPurchasedDocsQuery query, CancellationToken token)
            {
                const string sql = @"select DocumentId, D.Name as Title, U.Name as University, D.CourseName as Class, T.Price
                    from sb.[Transaction] T
                    join sb.Document D
	                    on T.DocumentId = D.Id
                    join sb.University U
	                    on D.UniversityId = U.Id
                    where User_Id = @Id and TransactionType = 'Document' and T.[Type] = 'Spent'
                    and User_Id in (select Id from sb.[user] where Id = User_Id and (Country = @Country or @Country is null))
                    order by 1
                    OFFSET @pageSize * @PageNumber ROWS
                    FETCH NEXT @pageSize ROWS ONLY;";
                using var connection = _dapper.OpenConnection();
                return await connection.QueryAsync<UserPurchasedDocsDto>(sql,
                    new
                    {
                        id = query.UserId,
                        PageNumber = query.Page,
                        PageSize,
                        query.Country
                    });
            }
        }
    }
}
