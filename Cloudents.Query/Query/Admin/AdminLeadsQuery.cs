using Cloudents.Core.DTOs.Admin;
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
        public AdminLeadsQuery()
        { }

        internal sealed class AdminLeadsQueryHandler : IQueryHandler<AdminLeadsQuery, IEnumerable<LeadDto>>
        {
            private readonly DapperRepository _dapper;


            public AdminLeadsQueryHandler(DapperRepository dapper)
            {
                _dapper = dapper;
            }

            public async Task<IEnumerable<LeadDto>> GetAsync(AdminLeadsQuery query, CancellationToken token)
            {
                const string sql = @"select l.Name, Email, Phone, Text, CourseId as Course, u.Name as University, Referer
from sb.Lead l
left join sb.University u
	on l.UniversityId = u.Id";
                using (var connection = _dapper.OpenConnection())
                {
                    var res = await connection.QueryAsync<LeadDto>(sql);
                    foreach (var item in res)
                    {
                        if (item.Referer.Contains("utm_source"))
                        {
                            var utm = item.Referer.Split('&', '?').Where(w => w.Contains("utm_source")).FirstOrDefault();
                            item.Referer = utm.Substring(utm.IndexOf('=')+1);
                        }
                        else
                        {
                            item.Referer = null;
                        }
                    }
                    return res;
                }
            }
        }
    }
}
