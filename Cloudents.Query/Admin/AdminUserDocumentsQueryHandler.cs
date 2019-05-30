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
    public class AdminUserDocumentsQueryHandler : IQueryHandler<AdminUserDocumentsQuery, IEnumerable<UserDocumentsDto>>
    {
        private readonly IStatelessSession _session;


        public AdminUserDocumentsQueryHandler(QuerySession session)
        {
            _session = session.StatelessSession;
        }
        private const int PageSize = 50;

        public async Task<IEnumerable<UserDocumentsDto>> GetAsync(AdminUserDocumentsQuery query, CancellationToken token)
        {

            return await _session.Query<Document>()
                  .Fetch(f => f.University)
                  .Where(w => w.User.Id == query.UserId)
                  .Select(s => new UserDocumentsDto
                  {
                      Course = s.Course.Id,
                      Id = s.Id,
                      Name = s.Name,
                      University = s.University.Name,
                      State = s.Status.State,
                      Price = s.Price,
                      Created = s.TimeStamp.CreationTime,

                  }).OrderBy(o => o.Id).Take(PageSize).Skip(PageSize * query.Page).ToListAsync(token);

        }
    }
}
