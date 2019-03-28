using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Interfaces;
using Cloudents.Query.Query.Admin;
using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Admin
{
    public class AdminUserDocumentsQueryHandler : IQueryHandler<AdminUserDocumentsQuery, IEnumerable<UserDocumentsDto>>
    {
        private readonly DapperRepository _dapper;


        public AdminUserDocumentsQueryHandler(DapperRepository dapper)
        {
            _dapper = dapper;
        }
        private const int PageSize = 200;

        public async Task<IEnumerable<UserDocumentsDto>> GetAsync(AdminUserDocumentsQuery query, CancellationToken token)
        {
            const string sql = @"select D.Id, D.Name, D.CreationTime as Created, U.Name as University, D.CourseName as Course, D.Price, D.[state]
                from sb.Document D
                join sb.University U
	                on D.UniversityId = U.Id
                where UserId = @Id
				order by 1
                 OFFSET @PageSize * @PageNumber ROWS
                 FETCH NEXT @PageSize ROWS ONLY;";
            return await _dapper.WithConnectionAsync(async connection =>
            {
                return await connection.QueryAsync<UserDocumentsDto>(sql,
                    new
                    {
                        id = query.UserId,
                        PageNumber = query.Page,
                        PageSize
                    });
            }, token);
        }
    }
}
