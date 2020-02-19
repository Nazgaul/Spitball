using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Persistence.Repositories
{
    public class CouponRepository : NHibernateRepository<Coupon>, ICouponRepository
    {
        public CouponRepository(ISession session) : base(session)
        {
        }

        public async Task<Coupon> GetCouponAsync(string coupon, CancellationToken token)
        {
            return await Session.Query<Coupon>().SingleOrDefaultAsync(x =>
                x.Code == coupon && x.Expiration.GetValueOrDefault(DateTime.UtcNow.AddDays(1)) > DateTime.UtcNow, cancellationToken: token);
        }
    }
}