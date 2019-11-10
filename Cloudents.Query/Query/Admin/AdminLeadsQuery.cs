using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query.Admin
{
    //TODO: think about make this suitable for IN.
    public class AdminLeadsQuery : IQueryAdmin<IEnumerable<LeadDto>>
    {
        private ItemState? Status { get; }
        public string Country { get; }
        public AdminLeadsQuery(ItemState? status, string country)
        {
            Status = status;
            Country = country;
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
                var leads = _session.Query<Lead>()
                    .Fetch(f => f.User)
                    .Where(w => w.Status == null || w.Status == query.Status);
                if (!string.IsNullOrEmpty(query.Country))
                {
                    leads = leads.Where(w => w.User.Country == query.Country);
                }

                return await leads.Select(s => new LeadDto
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
