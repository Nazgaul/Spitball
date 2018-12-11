using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using NHibernate;
using NHibernate.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Database.Query
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class UserProfileQueryHandler : IQueryHandler<UserDataByIdQuery, UserProfileDto>
    {
        private readonly ISession _session;

        public UserProfileQueryHandler(QuerySession session)
        {
            _session = session.Session;
        }

        public async Task<UserProfileDto> GetAsync(UserDataByIdQuery query, CancellationToken token)
        {
            return await _session.Query<User>()
                 .Fetch(u => u.University)
                 .Where(w => w.Id == query.Id)
                 .Select(s => new UserProfileDto
                 {
                         Id = s.Id,
                         Image = s.Image,
                         Name = s.Name,
                         UniversityName = s.University.Name
                 })
                 .SingleOrDefaultAsync(cancellationToken: token);

        }
    }
}