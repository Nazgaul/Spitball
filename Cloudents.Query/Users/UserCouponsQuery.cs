using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Users
{
    public class UserCouponsQuery : IQuery<IEnumerable<CouponDto>>
    {
        public UserCouponsQuery(long userId)
        {
            UserId = userId;
        }
        public long UserId { get; set; }

        internal sealed class UserCouponsQueryHandler : IQueryHandler<UserCouponsQuery, IEnumerable<CouponDto>>
        {
            private readonly IStatelessSession _session;

            public UserCouponsQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }

            public async Task<IEnumerable<CouponDto>> GetAsync(UserCouponsQuery query, CancellationToken token)
            {
                return await _session.Query<Coupon>()
                    .Where(w => w.Tutor.Id == query.UserId)
                    .Select(s => new CouponDto()
                    {
                        Code = s.Code,
                        CouponType = s.CouponType,
                        AmountOfUsers = s.AmountOfUsers,
                        CreateTime = s.CreateTime,
                        Expiration = s.Expiration,
                        Value = s.Value
                    }).ToListAsync(token);
            }
        }
    }
}
