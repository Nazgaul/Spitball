using System.Collections.Generic;
using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;

namespace Cloudents.Core.DTOs.Users
{
    public class UserTutorProfileDto
    {
        public decimal Price { get; set; }

        public string Currency => TutorCountry.RegionInfo.ISOCurrencySymbol;

        [EntityBind(nameof(ReadTutor.SbCountry))]
        public Country TutorCountry { get; set; }

        public decimal? DiscountPrice { get; set; }

        [EntityBind(nameof(ReadTutor.Rate))]
        public double Rate { get; set; }
        [EntityBind(nameof(ReadTutor.RateCount))]
        public int ReviewCount { get; set; }
       
        public bool HasCoupon { get; set; }

        public decimal? CouponValue { get; set; }
        public CouponType? CouponType { get; set; }

        [EntityBind(nameof(ReadTutor.Bio))] 
        public string Bio { get; set; }

        [EntityBind(nameof(ReadTutor.AllSubjects))]
        public IEnumerable<string>? Subjects { get; set; }

        [EntityBind(nameof(ReadTutor.Lessons))]
        public int Lessons { get; set; }

        public int ContentCount { get; set; }
        public int Students { get; set; }

        [EntityBind(nameof(User.Description))]
        public string Description { get; set; }
    }
}