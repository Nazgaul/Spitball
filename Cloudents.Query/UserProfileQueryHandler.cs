using Cloudents.Domain.Entities;
using NHibernate;
using NHibernate.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.DTOs;
using Cloudents.Application.Interfaces;
using Cloudents.Application.Query;

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
                         UniversityName = s.University.Name,
                         Score = s.Score
                 })
                 .SingleOrDefaultAsync(cancellationToken: token);

        }
    }
}