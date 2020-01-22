using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Admin
{
    public class FlaggedAnswerQuery : IQueryAdmin<IEnumerable<FlaggedAnswerDto>>
    {
        public string Country { get; set; }
        public FlaggedAnswerQuery(string country)
        {
            Country = country;
        }

        internal sealed class FlaggedAnswerEmptyQueryHandler : IQueryHandler<FlaggedAnswerQuery, IEnumerable<FlaggedAnswerDto>>
        {
            private readonly IStatelessSession _session;


            public FlaggedAnswerEmptyQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }

            public async Task<IEnumerable<FlaggedAnswerDto>> GetAsync(FlaggedAnswerQuery query, CancellationToken token)
            {
                var answers = _session.Query<Answer>()
                    .Fetch(f => f.User)
                    .Where(w => w.Status.State == ItemState.Flagged);
                if (!string.IsNullOrEmpty(query.Country))
                {
                    answers = answers.Where(w => w.User.Country == query.Country);
                }

                return await answers.Select(s => new FlaggedAnswerDto
                {
                    Id = s.Id,
                    Text = s.Text,
                    Reason = s.Status.FlagReason,
                    FlaggedUserEmail = s.User.Email,
                    QuestionId = s.Question.Id,
                    QuestionText = s.Question.Text,
                    MarkerEmail = s.Status.FlaggedUser.Email
                }).OrderBy(o => o.Id).ToListAsync(token);
            }
        }
    }
}
