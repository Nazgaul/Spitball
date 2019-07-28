using Cloudents.Core.DTOs.Admin;
using Cloudents.Query.Query.Admin;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query.Admin
{
    public class AdminUserQuestionsQueryHandler : IQueryHandler<AdminUserQuestionsQuery, IEnumerable<UserQuestionsDto>>
    {
        private readonly IStatelessSession _session;

        public AdminUserQuestionsQueryHandler(QuerySession session)
        {
            _session = session.StatelessSession;
        }
        private const int PageSize = 200;

        public async Task<IEnumerable<UserQuestionsDto>> GetAsync(AdminUserQuestionsQuery query, CancellationToken token)
        {

          return  await _session.Query<Question>()
                .Where(w => w.User.Id == query.UserId && w.User.Country == query.Country)
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
