using System.Linq;
using Cloudents.Core.DTOs;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;
using System;

namespace Cloudents.Query.Tutor
{
    public class UserEmailInfoQuery: IQuery<UserEmailInfoDto>
    {
        public UserEmailInfoQuery(long userId)
        {
            UserId = userId;
        }

        private long UserId { get;  }

        internal sealed class UserEmailInfoQueryHandler : IQueryHandler<UserEmailInfoQuery, UserEmailInfoDto>
        {
            private readonly IStatelessSession _statelessSession;
            public UserEmailInfoQueryHandler(QuerySession querySession)
            {
                _statelessSession = querySession.StatelessSession;
            }
            public async Task<UserEmailInfoDto> GetAsync(UserEmailInfoQuery query, CancellationToken token)
            {
                var res = _statelessSession.Query<User>()
                    .Where(w => w.Id == query.UserId)
                    .Select(s => new UserEmailInfoDto()
                    {
                        Name = s.Name, Email = s.Email, University = s.University.Id, PhoneNumber = s.PhoneNumber
                    }).ToFuture();//.SingleOrDefaultAsync(token);

                var leadCount = _statelessSession.Query<Lead>()
                    .Where(w => w.User.Id == query.UserId && w.CreationTime > DateTime.UtcNow.AddDays(-1))
                    .ToFuture();

                var result = res.SingleOrDefault();
                result.LeadCount = leadCount.Count();
                return await Task.FromResult(result);
            }
        }
    }
}
