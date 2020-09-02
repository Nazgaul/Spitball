using Cloudents.Core.Entities;
using System;
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs
{
    public class CouponDto
    {
        public string Code { get; set; }
        public CouponType CouponType { get; set; }
        public decimal Value { get; set; }
        public int AmountOfUsers { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? Expiration { get; set; }
    }
}
