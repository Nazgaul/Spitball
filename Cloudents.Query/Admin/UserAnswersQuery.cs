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
    public class UserAnswersQuery : IQueryAdmin<IEnumerable<UserAnswersDto>>
    {
        public UserAnswersQuery(long userId, int page, string country)
        {
            UserId = userId;
            Page = page;
            Country = country;
        }

        private long UserId { get; }
        private int Page { get; }
        public string Country { get; }
        internal sealed class UserAnswersQueryHandler : IQueryHandler<UserAnswersQuery, IEnumerable<UserAnswersDto>>
        {
            private readonly IStatelessSession _session;

            public UserAnswersQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }
            private const int PageSize = 200;

            public async Task<IEnumerable<UserAnswersDto>> GetAsync(UserAnswersQuery query, CancellationToken token)
            {
                return await _session.Query<Answer>()
                    .WithOptions(w => w.SetComment(nameof(UserAnswersQuery)))
                     //  .Fetch(f => f.Question)
                     .Where(w => w.User.Id == query.UserId)
                     .Where(w => w.User.Country == query.Country || string.IsNullOrEmpty(query.Country))
                     .Take(PageSize).Skip(PageSize * query.Page)
                     .OrderBy(o => o.Id)
                     .Select(s => new UserAnswersDto
                     {
                         Id = s.Id,
                         State = s.Status.State,
                         Created = s.Created,
                         QuestionId = s.Question.Id,
                         QuestionText = s.Question.Text,
                         Text = s.Text
                     }).ToListAsync(token);
              
            }
        }
    }
}