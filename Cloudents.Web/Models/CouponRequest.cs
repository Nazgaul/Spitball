using Cloudents.Core.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class CouponRequest
    {
        /// <summary>
        /// The value of the coupon
        /// </summary>
        [Required]
        public decimal Value { get; set; }

        /// <summary>
        /// Code of the coupon - e.g Friends10
        /// </summary>
        [Required, StringLength(Coupon.MaxLength, MinimumLength = Coupon.MinimumLength)]
        public string Code { get; set; }

        /// <summary>
        /// Type of coupon - flat or percentage
        /// </summary>
        [Required]
        public CouponType CouponType { get; set; }

        [Required]
        public DateTime Expiration { get; set; }
    }
}
