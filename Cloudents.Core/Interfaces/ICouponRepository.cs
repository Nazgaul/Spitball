using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;

namespace Cloudents.Core.Interfaces
{
    public interface ICouponRepository : IRepository<Coupon>
    {
        Task<Coupon> GetCouponAsync(string coupon, CancellationToken token);
    }
}