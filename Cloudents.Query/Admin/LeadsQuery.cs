using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Admin
{
    //TODO: think about make this suitable for IN.
    public class LeadsQuery : IQueryAdmin<IEnumerable<LeadDto>>
    {
        public string Country { get; }
        public LeadsQuery(string country)
        {
            Country = country;
        }

        internal sealed class LeadsQueryHandler : IQueryHandler<LeadsQuery, IEnumerable<LeadDto>>
        {
            private readonly IStatelessSession _session;


            public LeadsQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }

            public async Task<IEnumerable<LeadDto>> GetAsync(LeadsQuery query, CancellationToken token)
            {
                //User userAlias = null;
                //Lead leadAlias = null;
                

                //_session.QueryOver<Lead>(() => leadAlias)
                //    .JoinAlias(f => f.User, () => userAlias)
                //    .JoinEntityAlias(() => userCourseAlias, () => leadAlias.Id == viewTutorAlias.Id)

                //    .JoinQueryOver(x=>x.Id,)

                IQueryable<Lead> leads = _session.Query<Lead>()
                    .Fetch(f => f.User)
                    .Where(w=> !_session.Query<ChatRoomAdmin>().Any(w2=>w2.Lead.Id == w.Id));

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
                    Course = s.Course,
                    University = s.User.University.Name,
                    DateTime = s.CreationTime 
                    //Referer = s.UtmSource,
                    //Status = s.Status
                }).OrderByDescending(o=>o.Id).ToListAsync(token);

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
