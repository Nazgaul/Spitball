using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using System;

namespace Cloudents.Core.DTOs.Admin
{
    public class PaymentDto
    {
        [EntityBind(nameof(StudyRoomSession.Id))]
        public Guid StudyRoomSessionId { get; set; }
        [EntityBind(nameof(Tutor.Price))]
        public decimal Price { get; set; }

        [EntityBind(nameof(Tutor.SellerKey))]
        public bool CantPay { get; set; }
        [EntityBind(nameof(Tutor.Id))]
        public long TutorId { get; set; }
        [EntityBind(nameof(User.Name))]
        public string TutorName { get; set; }
        [EntityBind(nameof(User.Id))]
        public long UserId { get; set; }
        [EntityBind(nameof(User.Name))]
        public string UserName { get; set; }
        [EntityBind(nameof(StudyRoomSession.Created))]
        public DateTime Created { get; set; }
        [EntityBind(nameof(StudyRoomSession.Created), nameof(StudyRoomSession.Ended))]
        public TimeSpan Duration { get; set; }
    }

    public class PaymentDetailDto
    {
        [EntityBind(nameof(StudyRoomSession.Id))]
        public Guid StudyRoomSessionId { get; set; }
        [EntityBind(nameof(Tutor.Price))]
        public decimal TutorPricePerHour { get; set; }

        
        public decimal StudentPayPerHour { get; set; }
        public decimal SpitballPayPerHour { get; set; }


        [EntityBind(nameof(Tutor.SellerKey))]
        public bool CantPay { get; set; }
        [EntityBind(nameof(Tutor.Id))]
        public long TutorId { get; set; }
        [EntityBind(nameof(User.Name))]
        public string TutorName { get; set; }
        [EntityBind(nameof(User.Id))]
        public long UserId { get; set; }
        [EntityBind(nameof(User.Name))]
        public string UserName { get; set; }
        [EntityBind(nameof(StudyRoomSession.Created))]
        public DateTime Created { get; set; }
        [EntityBind(nameof(StudyRoomSession.Created), nameof(StudyRoomSession.Ended))]
        public int Duration { get; set; }

        [EntityBind(nameof(Coupon.Code))]
        public string CouponCode { get; set; }
        [EntityBind(nameof(Coupon.CouponType))]
        public CouponType? CouponType { get; set; }
        [EntityBind(nameof(Coupon.Value))]
        public decimal? CouponValue { get; set; }

        [EntityBind(nameof(Coupon.Tutor.Id))]
        public long? CouponTutor { get; set; }
        
    }
}
