using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class UseCouponEvent : IEvent
    {
        public UseCouponEvent(UserCoupon userCoupon)
        {
            UserCoupon = userCoupon;
        }
        public UserCoupon UserCoupon { get; set; }

    }

    public class ApplyCouponEvent : IEvent
    {
        public ApplyCouponEvent(UserCoupon userCoupon)
        {
            UserCoupon = userCoupon;
        }
        public UserCoupon UserCoupon { get; set; }
    }
}
