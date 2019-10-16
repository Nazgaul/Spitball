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
    public class AdminPendingQuestionsQuery : IQueryAdmin<IEnumerable<PendingQuestionDto>>
    {
        public AdminPendingQuestionsQuery(string country)
        {
            Country = country;
        }
        public string Country { get; }
        internal sealed class AdminPendingQuestionsEmptyQueryHandler : IQueryHandler<AdminPendingQuestionsQuery, IEnumerable<PendingQuestionDto>>
        {
            private readonly IStatelessSession _session;


            public AdminPendingQuestionsEmptyQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }

            public async Task<IEnumerable<PendingQuestionDto>> GetAsync(AdminPendingQuestionsQuery query, CancellationToken token)
            {
                var questions = _session.Query<Question>()
                    .Fetch(f => f.User)
                    .Where(w => w.User is User && w.Status.State == ItemState.Pending);

                if (!string.IsNullOrEmpty(query.Country))
                {
                    questions = questions.Where(w => w.User.Country == query.Country);
                }
                    
                return await questions.Select(s => new PendingQuestionDto
                    {
                        Id = s.Id,
                        Text = s.Text,
                        Email = s.User.Email,
                        UserId = s.User.Id,
                    }).OrderBy(o => o.Id).ToListAsync(token);
            }
        }
    }
}
