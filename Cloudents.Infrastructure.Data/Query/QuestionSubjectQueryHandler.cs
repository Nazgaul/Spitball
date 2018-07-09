using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using NHibernate;
using System.Linq;
using NHibernate.Linq;

namespace Cloudents.Infrastructure.Data.Query
{
    public class QuestionSubjectQueryHandler : IQueryHandler<QuestionSubjectQuery, IEnumerable<QuestionSubjectDto>>
    {
        private readonly IStatelessSession _session;

        public QuestionSubjectQueryHandler(ReadonlyStatelessSession session)
        {
            _session = session.Session;
        }


        public async Task<IEnumerable<QuestionSubjectDto>> GetAsync(QuestionSubjectQuery query, CancellationToken token)
        {
            return await _session.Query<QuestionSubject>()
                .Select(s => new QuestionSubjectDto
                {
                    Id = s.Id,
                    Subject = s.Text
                })
                .OrderBy(o => o.Subject)
                .ToListAsync(token).ConfigureAwait(false);
        }
    }
}