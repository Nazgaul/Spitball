using System.Collections.Generic;
using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;

namespace Cloudents.Core.DTOs.Users
{
    public class UserTutorProfileDto
    {
        public Country? TutorCountry { get; set; }


        public double Rate { get; set; }
        public int ReviewCount { get; set; }

        public bool HasCoupon { get; set; }

        public decimal? CouponValue { get; set; }
        public CouponType? CouponType { get; set; }

        public string? Bio { get; set; }

       // public IEnumerable<string>? Subjects { get; set; }

        public int Lessons { get; set; }

        public int ContentCount { get; set; }
        public int Students { get; set; }

        public Money? SubscriptionPrice { get; set; }

        public bool IsSubscriber { get; set; }

        public string? Description { get; set; }



    }
}