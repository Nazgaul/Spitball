using Cloudents.Core.DTOs.Admin;
using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query.Admin
{
    public class AdminPendingTutorsQuery: IQuery<IEnumerable<PendingTutorsDto>>
    {
        internal sealed class AdminPendingTutorsQueryHandler : IQueryHandler<AdminPendingTutorsQuery, IEnumerable<PendingTutorsDto>>
        {
            private readonly DapperRepository _repository;

            public AdminPendingTutorsQueryHandler(DapperRepository repository)
            {
                _repository = repository;
            }

            public async Task<IEnumerable<PendingTutorsDto>> GetAsync(AdminPendingTutorsQuery query, CancellationToken token)
            {
                const string sql = @"select t.Id, u.FirstName, u.LastName, t.Bio, t.Price, u.Email
                                    from sb.Tutor t
                                    join sb.[User] u
	                                    on t.Id = u.Id
                                    where t.State = 'Pending'";
                using (var conn = _repository.OpenConnection())
                {
                    var retVal = await conn.QueryAsync<PendingTutorsDto>(sql);

                    return retVal;
                }
            }
        }
    }
}
