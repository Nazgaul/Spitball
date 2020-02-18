using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using System;

namespace Cloudents.Core.DTOs
{
    public class CouponDto
    {
        [EntityBind(nameof(Coupon.Code))]
        public string Code { get; set; }
        [EntityBind(nameof(Coupon.CouponType))]
        public CouponType CouponType { get; set; }
        [EntityBind(nameof(Coupon.Value))]
        public decimal Value { get; set; }
        [EntityBind(nameof(Coupon.AmountOfUsers))]
        public int? AmountOfUsers { get; set; }
        [EntityBind(nameof(Coupon.CreateTime))]
        public DateTime? CreateTime { get; set; }
        [EntityBind(nameof(Coupon.Expiration))]
        public DateTime? Expiration { get; set; }
    }
}
