using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Transform;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Users
{
    public class UserFollowersByIdQuery : IQuery<IEnumerable<FollowersDto>>
    {
        public UserFollowersByIdQuery(long userId)
        {
            UserId = userId;
        }
        public long UserId { get; set; }

        internal sealed class UserFollowersByIdQueryHandler : IQueryHandler<UserFollowersByIdQuery, IEnumerable<FollowersDto>>
        {
            private readonly IStatelessSession _session;

            public UserFollowersByIdQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }

            public async Task<IEnumerable<FollowersDto>> GetAsync(UserFollowersByIdQuery query, CancellationToken token)
            {
                User userAlias = null;
                Follow followAlias = null;
                User followerAlias = null;

                FollowersDto resultAlias = null;

                var res = await _session.QueryOver(() => userAlias)
                    .JoinAlias(x => x.Followers, () => followAlias)
                    .JoinEntityAlias(() => followerAlias, () => followAlias.Follower.Id == followerAlias.Id)
                    .Where(w => w.Id == query.UserId)
                    .SelectList(sl =>
                    sl.Select(() => followerAlias.Id).WithAlias(() => resultAlias.UserId)
                    .Select(() => followerAlias.Name).WithAlias(() => resultAlias.Name)
                    .Select(() => followerAlias.ImageName).WithAlias(() => resultAlias.Image)
                    .Select(() => followerAlias.Email).WithAlias(() => resultAlias.Email)
                    .Select(() => followerAlias.PhoneNumber).WithAlias(() => resultAlias.PhoneNumber)
                    .Select(() => followAlias.Created).WithAlias(() => resultAlias.Created)
                    )
                    .TransformUsing(Transformers.AliasToBean<FollowersDto>())
                     .ListAsync<FollowersDto>(token);

                return res;
            }
        }
    }
}
