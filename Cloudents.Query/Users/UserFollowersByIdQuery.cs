using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using NHibernate;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Linq;
using PaymentStatus = Cloudents.Core.Enum.PaymentStatus;

namespace Cloudents.Query.Users
{
    public class UserFollowersByIdQuery : IQuery<IEnumerable<FollowersDto>>
    {
        public UserFollowersByIdQuery(long userId)
        {
            UserId = userId;
        }
        private long UserId { get; set; }

        internal sealed class UserFollowersByIdQueryHandler : IQueryHandler<UserFollowersByIdQuery, IEnumerable<FollowersDto>>
        {
            private readonly IStatelessSession _session;

            public UserFollowersByIdQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }

            public async Task<IEnumerable<FollowersDto>> GetAsync(UserFollowersByIdQuery query, CancellationToken token)
            {
                return await _session.Query<Follow>()
                       .Fetch(f => f.Follower)
                       .Where(w => w.User.Id == query.UserId)
                       .Select(s => new FollowersDto()
                       {
                           Email = s.Follower.Email,
                           Created = s.Created,
                           Name = s.Follower.Name,
                           PhoneNumber = s.Follower.PhoneNumber,
                           Image = s.Follower.ImageName,
                           UserId = s.Follower.Id,
                           HasCreditCard = s.Follower.PaymentExists == PaymentStatus.Done
                       }).ToListAsync(token);

            }
        }
    }
}
