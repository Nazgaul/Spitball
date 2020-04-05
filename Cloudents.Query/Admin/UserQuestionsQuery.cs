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
    public class UserQuestionsQuery : IQueryAdmin<IEnumerable<UserQuestionsDto>>
    {
        public UserQuestionsQuery(long userId, int page, string country)
        {
            UserId = userId;
            Page = page;
            Country = country;
        }
        private long UserId { get; }
        private int Page { get; }
        public string Country { get; }
        internal sealed class UserQuestionsQueryHandler : IQueryHandler<UserQuestionsQuery, IEnumerable<UserQuestionsDto>>
        {
            private readonly IStatelessSession _session;

            public UserQuestionsQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }
            private const int PageSize = 200;

            public async Task<IEnumerable<UserQuestionsDto>> GetAsync(UserQuestionsQuery query, CancellationToken token)
            {

                return await _session.Query<Question>()
                    .WithOptions(w => w.SetComment(nameof(UserQuestionsQuery)))
                        .Where(w => w.User.Id == query.UserId)
                        .Where(w => w.User.Country == query.Country || string.IsNullOrEmpty(query.Country))
                        .Take(PageSize).Skip(PageSize * query.Page)
                        .Select(s => new UserQuestionsDto
                        {
                            Id = s.Id,
                            Text = s.Text,
                            State = s.Status.State,
                            Created = s.Created
                        }).ToListAsync(token);
            }
        }
    }
}
