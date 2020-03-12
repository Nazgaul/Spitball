using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;

namespace Cloudents.Core.DTOs
{
    public class PaymentDetailDto
    {
        [EntityBind(nameof(Tutor.Price))]
        public decimal TutorPricePerHour { get; set; }
        [EntityBind(nameof(Coupon.Code))]
        public string? CouponCode { get; set; }
        [EntityBind(nameof(Coupon.CouponType))]
        public CouponType? CouponType { get; set; }
        [EntityBind(nameof(Coupon.Value))]
        public decimal? CouponValue { get; set; }

        [EntityBind(nameof(Coupon.Tutor.Id))]
        public long? CouponTutor { get; set; }

    }
}
