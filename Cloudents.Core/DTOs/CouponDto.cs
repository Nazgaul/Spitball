using Cloudents.Core.Entities;
using System;

namespace Cloudents.Core.DTOs
{
    public class CouponDto
    {
        public string Code { get; set; }
        public CouponType CouponType { get; set; }
        public double Value { get; set; }
        public int AmountOfUsers { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? Expiration { get; set; }
    }
}
