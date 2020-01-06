using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query.Query.Admin
{
    public class AdminCouponQuery : IQuery<IEnumerable<CouponDto>>
    {
        internal sealed class
            AdminAllConversationsQueryHandler : IQueryHandler<AdminCouponQuery, IEnumerable<CouponDto>>
        {
            private readonly IStatelessSession _statelessSession;

            public AdminAllConversationsQueryHandler(QuerySession statelessSession)
            {
                _statelessSession = statelessSession.StatelessSession;
            }

            public async Task<IEnumerable<CouponDto>> GetAsync(AdminCouponQuery query, CancellationToken token)
            {
                return  await _statelessSession.Query<Coupon>()
                     .Select(s => new CouponDto()
                     {
                         Value = s.Value,
                         AmountOfUsers = _statelessSession.Query<UserCoupon>().Count(w => w.Coupon.Id == s.Id),
                         Code = s.Code,
                         CouponType = s.CouponType,
                         TutorId = s.Tutor.Id,
                         Description = s.Description,
                         Owner = s.Owner
                     }).ToListAsync(token);
            }
        }
    }
}