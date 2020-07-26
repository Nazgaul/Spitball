using Cloudents.Core.DTOs.Admin;
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
    public class PendingTutorsQuery : IQueryAdmin2<IEnumerable<PendingTutorsDto>>
    {
        public PendingTutorsQuery(Country? country)
        {
            Country = country;
        }

        public Country? Country { get; }

        internal sealed class PendingTutorsQueryHandler : IQueryHandler<PendingTutorsQuery, IEnumerable<PendingTutorsDto>>
        {
            private readonly IStatelessSession _session;
            public PendingTutorsQueryHandler(IStatelessSession session)
            {
                _session = session;
            }

            public async Task<IEnumerable<PendingTutorsDto>> GetAsync(PendingTutorsQuery query, CancellationToken token)
            {
                var tutorQuery = _session.Query<Core.Entities.Tutor>()
                   // .Fetch(f=>f.User)
                   // .ThenFetchMany(f=>f.UserCourses)
                    .Where(w => w.State == ItemState.Pending);

                if (query.Country != null)
                {
                    tutorQuery = tutorQuery.Where(w => w.User.SbCountry == query.Country);
                }
                //THIS Query is not optimum
                return await tutorQuery.OrderByDescending(t=>t.Id).Select(s => new PendingTutorsDto
                {
                    FirstName = s.User.FirstName,
                    Email = s.User.Email,
                    LastName = s.User.LastName,
                    Bio = s.Paragraph2,
                    Image = s.User.ImageName,
                    Created = s.Created,
                    Id = s.Id,
                    Courses2 = s.Courses.Select(s2 => s2.Name)
                }).ToListAsync(token);


            }
        }
    }
}
