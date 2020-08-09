using Cloudents.Core.Entities;

namespace Cloudents.Core.DTOs
{
    public class PaymentDetailDto
    {
        public double TutorPricePerHour { get; set; }
        public string? CouponCode { get; set; }
        public CouponType? CouponType { get; set; }
        public double? CouponValue { get; set; }

        public long? CouponTutor { get; set; }

    }
}
