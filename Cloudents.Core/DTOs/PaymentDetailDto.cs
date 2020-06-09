using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;

namespace Cloudents.Core.DTOs
{
    public class PaymentDetailDto
    {
        public double TutorPricePerHour { get; set; }
        public string? CouponCode { get; set; }
        public CouponType? CouponType { get; set; }
        public decimal? CouponValue { get; set; }

        public long? CouponTutor { get; set; }

    }
}
