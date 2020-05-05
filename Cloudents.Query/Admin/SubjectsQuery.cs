using Cloudents.Core.DTOs.Admin;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query.Admin
{
    public class SubjectsQuery : IQueryAdmin2<IEnumerable<SubjectDto>>
    {
        public SubjectsQuery(Country? country)
        {
            Country = country;
        }

        public Country? Country { get; }

        internal sealed class SubjectsTranslationQueryHandler : IQueryHandler<SubjectsQuery, IEnumerable<SubjectDto>>
        {
            private readonly IStatelessSession _statelessSession;
            public SubjectsTranslationQueryHandler(QuerySession dapperRepository)
            {
                _statelessSession = dapperRepository.StatelessSession;
            }

            public async Task<IEnumerable<SubjectDto>> GetAsync(SubjectsQuery query,
                CancellationToken token)
            {

                var x = _statelessSession.Query<CourseSubject>();
                if (query.Country != null)
                {
                    x = x.Where(w => w.Country == query.Country);
                }
                return await x.Select(s => new SubjectDto
                {
                    Id = s.Id,
                    Name = s.Name
                }).ToListAsync(token);
            }
        }

    }
}
