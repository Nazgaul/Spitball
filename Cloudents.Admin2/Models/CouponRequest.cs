using System;
using System.ComponentModel.DataAnnotations;
using Cloudents.Core.Entities;

namespace Cloudents.Admin2.Models
{
    public class CouponRequest
    {

        /// <summary>
        /// meta description
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// meta description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// optional - When the coupon expire
        /// </summary>
        public DateTime? Expiration { get; set; }


        /// <summary>
        /// optinal - The amount of coupons
        /// </summary>
        public int? Amount { get; set; }


        /// <summary>
        /// optinal - The amount of coupons
        /// </summary>
        public int? UsePerUser { get; set; }

        /// <summary>
        /// The value of the coupon
        /// </summary>
        [Required]
        public decimal Value { get; set; }

        /// <summary>
        /// optional - if the coupon belong to specific tutor if so the tutor id
        /// </summary>
        public long? TutorId { get; set; }
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
    }
}