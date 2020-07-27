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
        public UserDocumentsQuery(long userId, Country? country)
        {
            UserId = userId;
            Country = country;
        }
        private long UserId { get; }
        public Country? Country { get; }
        internal sealed class UserDocumentsQueryHandler : IQueryHandler<UserDocumentsQuery, IEnumerable<UserDocumentsDto>>
        {
            private readonly IStatelessSession _session;


            public UserDocumentsQueryHandler(IStatelessSession session)
            {
                _session = session;
            }

            public async Task<IEnumerable<UserDocumentsDto>> GetAsync(UserDocumentsQuery query, CancellationToken token)
            {

                var dbQuery = _session.Query<Document>()
                    .WithOptions(w => w.SetComment(nameof(UserDocumentsQuery)))
                        .Where(w => w.User.Id == query.UserId);
                if (query.Country != null)
                {
                    dbQuery = dbQuery.Where(w => w.User.SbCountry == query.Country);
                }

                return await dbQuery.Select(s => new UserDocumentsDto
                {
                    Course = s.Course.Name,
                    Id = s.Id,
                    Name = s.Name,
                    State = s.Status.State,
                    Price = s.DocumentPrice.Price,
                    Created = s.TimeStamp.CreationTime,

                }).OrderBy(o => o.Id)
                     .ToListAsync(token);
            }
        }
    }
}
