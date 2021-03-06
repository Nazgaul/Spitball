﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query.Admin
{
    public class CouponQuery : IQuery<IEnumerable<CouponDto>>
    {
        internal sealed class CouponQueryHandler : IQueryHandler<CouponQuery, IEnumerable<CouponDto>>
        {
            private readonly IStatelessSession _statelessSession;

            public CouponQueryHandler(IStatelessSession statelessSession)
            {
                _statelessSession = statelessSession;
            }

            public async Task<IEnumerable<CouponDto>> GetAsync(CouponQuery query, CancellationToken token)
            {
                return  await _statelessSession.Query<Coupon>()
                    .WithOptions(w => w.SetComment(nameof(CouponQuery)))
                     .Select(s => new CouponDto()
                     {
                         //Temp for now
                         Value  = s.Value == null ? 0 : s.Value,
                         AmountOfUsers = _statelessSession.Query<UserCoupon>().Count(w => w.Coupon.Id == s.Id),
                         Code = s.Code,
                         CouponType = s.CouponType,
                         TutorId = s.Tutor!.Id,
                         Description = s.Description,
                     }).ToListAsync(token);
            }
        }
    }
}