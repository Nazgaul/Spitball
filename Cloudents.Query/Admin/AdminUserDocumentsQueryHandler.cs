using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities;
using Cloudents.Query.Query.Admin;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Admin
{
    public class AdminUserDocumentsQueryHandler : IQueryHandler<AdminUserDocumentsQuery, IEnumerable<UserDocumentsDto>>
    {
        private readonly IStatelessSession _session;


        public AdminUserDocumentsQueryHandler(QuerySession session)
        {
            _session = session.StatelessSession;
        }
        private const int PageSize = 25;

        public async Task<IEnumerable<UserDocumentsDto>> GetAsync(AdminUserDocumentsQuery query, CancellationToken token)
        {

            return await _session.Query<Document>()
                  .Fetch(f => f.University)
                  .Where(w => w.User.Id == query.UserId)
                  .Where(w => w.User.Country == query.Country || string.IsNullOrEmpty(query.Country))
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
