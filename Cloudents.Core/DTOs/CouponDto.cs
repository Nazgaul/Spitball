using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using System;

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
