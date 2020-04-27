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

    public class LeadsQuery : IQueryAdmin2<IEnumerable<LeadDto>>
    {
        public Country? Country { get; }
        public LeadsQuery(Country? country)
        {
            Country = country;
        }

        internal sealed class LeadsQueryHandler :
            IQueryHandler<LeadsQuery, IEnumerable<LeadDto>>
        {
            private readonly IStatelessSession _session;

            public LeadsQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }

            public async Task<IEnumerable<LeadDto>> GetAsync(LeadsQuery query, CancellationToken token)
            {
                var leads = _session.Query<Lead>()
                    .WithOptions(w => w.SetComment(nameof(LeadsQuery)))
                    .Fetch(f => f.User)
                    .Where(w => !_session.Query<ChatRoomAdmin>().Any(w2 => w2.Lead!.Id == w.Id));

                if (query.Country != null)
                {
                    leads = leads.Where(w => w.User.SbCountry == query.Country);
                }

                return await leads.Select(s => new LeadDto
                {
                    Id = s.Id,
                    Name = s.User.Name,
                    Email = s.User.Email,
                    Phone = s.User.PhoneNumber,
                    Text = s.Text,
                    Course = s.Course,
                    DateTime = s.CreationTime
                }).OrderByDescending(o => o.Id).ToListAsync(token);
            }
        }
    }
}
