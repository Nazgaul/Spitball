using Cloudents.Core.DTOs.Admin;
using Cloudents.Query.Query.Admin;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Admin
{
    public class AdminUserPurchasedDocsQueryHandler : IQueryHandler<AdminUserPurchasedDocsQuery, IEnumerable<UserPurchasedDocsDto>>
    {
        private readonly DapperRepository _dapper;


        public AdminUserPurchasedDocsQueryHandler(DapperRepository dapper)
        {
            _dapper = dapper;
        }
        private const int PageSize = 200;


        public async Task<IEnumerable<UserPurchasedDocsDto>> GetAsync(AdminUserPurchasedDocsQuery query, CancellationToken token)
        {
            const string sql = @"select DocumentId, D.Name as Title, U.Name as University, D.CourseName as Class, T.Price
                from sb.[Transaction] T
                join sb.Document D
	                on T.DocumentId = D.Id
                join sb.University U
	                on D.UniversityId = U.Id
                where User_Id = @Id and TransactionType = 'Document' and T.[Type] = 'Spent'
                order by 1
                OFFSET @pageSize * @PageNumber ROWS
                FETCH NEXT @pageSize ROWS ONLY;";
            using (var connection = _dapper.OpenConnection())
            {
                return await connection.QueryAsync<UserPurchasedDocsDto>(sql,
                    new
                    {
                        id = query.UserId,
                        PageNumber = query.Page,
                        PageSize
                    });
            }
        }
    }
}
