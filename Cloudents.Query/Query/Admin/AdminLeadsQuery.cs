using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Dapper;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Linq;

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
            private readonly IStatelessSession _session;


            public AdminLeadsQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }

            public async Task<IEnumerable<LeadDto>> GetAsync(AdminLeadsQuery query, CancellationToken token)
            {
                return await _session.Query<Lead>()
                    .Fetch(f=>f.User)
                    .Where(w => w.Status == null || w.Status == query.Status)
                     .Select(s => new LeadDto
                     {
                         Id = s.Id,
                         Name = s.User.Name,
                         Email = s.User.Email,
                         Phone = s.User.PhoneNumber,
                         Text = s.Text,
                         Course = s.Course.Id,
                         University = s.User.University.Name,
                         Referer = s.UtmSource,
                         Status = s.Status
                     }).ToListAsync(token);

                //                const string sql = @"select l.Id, l.Name, Email, Phone, Text, CourseId as Course, u.Name as University, UtmSource as Referer, l.Status
                //from sb.Lead l
                //left join sb.University u
                //	on l.UniversityId = u.Id
                //where l.Status = @Status or @Status = ''";
                //                using (var connection = _dapper.OpenConnection())
                //                {
                //                    var res = await connection.QueryAsync<LeadDto>(sql, new { Status = query.Status.ToString() });

                //                    return res;
                //                }
            }
        }
    }
}
