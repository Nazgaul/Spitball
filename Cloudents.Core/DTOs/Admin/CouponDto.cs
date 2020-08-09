using Cloudents.Core.Entities;

namespace Cloudents.Core.DTOs.Admin
{
    public class CouponDto
    {
        public string Code { get; set; }
        public CouponType CouponType { get; set; }
        public double Value { get; set; }
        public long? TutorId { get; set; }
        public string? Description { get;  set; }

      //  public string Owner { get;  set; }

        public int AmountOfUsers { get; set; }
    }
}