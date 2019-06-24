using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Enum;
using Dapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query.Admin
{
    public class AdminLeadsQuery: IQuery<IEnumerable<LeadDto>>
    {
        public ItemState? Status { get; }
        public AdminLeadsQuery(ItemState? status)
        {
            Status = status;
        }

        internal sealed class AdminLeadsQueryHandler : IQueryHandler<AdminLeadsQuery, IEnumerable<LeadDto>>
        {
            private readonly DapperRepository _dapper;


            public AdminLeadsQueryHandler(DapperRepository dapper)
            {
                _dapper = dapper;
            }

            public async Task<IEnumerable<LeadDto>> GetAsync(AdminLeadsQuery query, CancellationToken token)
            {
                const string sql = @"select l.Id, l.Name, Email, Phone, Text, CourseId as Course, u.Name as University, UtmSource as Referer, l.Status
from sb.Lead l
left join sb.University u
	on l.UniversityId = u.Id
where l.Status = @Status or @Status = ''";
                using (var connection = _dapper.OpenConnection())
                {
                    var res = await connection.QueryAsync<LeadDto>(sql, new { Status = query.Status.ToString() });
                    
                    return res;
                }
            }
        }
    }
}
