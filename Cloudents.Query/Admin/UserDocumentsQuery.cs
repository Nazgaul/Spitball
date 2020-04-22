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
    public class UserDocumentsQuery : IQueryAdmin2<IEnumerable<UserDocumentsDto>>
    {
        public UserDocumentsQuery(long userId, int page, Country? country)
        {
            UserId = userId;
            Page = page;
            Country = country;
        }
        private long UserId { get; }
        private int Page { get; }
        public Country? Country { get; }
        internal sealed class UserDocumentsQueryHandler : IQueryHandler<UserDocumentsQuery, IEnumerable<UserDocumentsDto>>
        {
            private readonly IStatelessSession _session;


            public UserDocumentsQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }
            private const int PageSize = 25;

            public async Task<IEnumerable<UserDocumentsDto>> GetAsync(UserDocumentsQuery query, CancellationToken token)
            {

                var dbQuery = _session.Query<Document>()
                    .WithOptions(w => w.SetComment(nameof(UserDocumentsQuery)))
                        .Fetch(f => f.University)
                        .Where(w => w.User.Id == query.UserId);
                if (query.Country != null)
                {
                    dbQuery = dbQuery.Where(w => w.User.SbCountry == query.Country);
                }

                return await dbQuery.Select(s => new UserDocumentsDto
                {
                    Course = s.Course.Id,
                    Id = s.Id,
                    Name = s.Name,
                    State = s.Status.State,
                    Price = s.Price,
                    Created = s.TimeStamp.CreationTime,

                }).OrderBy(o => o.Id)
                     .Take(PageSize)
                     .Skip(PageSize * query.Page)
                     .ToListAsync(token);
            }
        }
    }
}
