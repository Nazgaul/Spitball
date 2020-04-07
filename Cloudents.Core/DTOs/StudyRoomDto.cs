using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using System;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Dapper")]
    [SuppressMessage("Style", "CS8618", Justification = "Dapper")]
    public class StudyRoomDto
    {

        [EntityBind(nameof(StudyRoom.OnlineDocumentUrl))]
        public string OnlineDocument { get; set; }
        [EntityBind(nameof(ChatRoom.Id))]
        public string ConversationId { get; set; }
        [EntityBind(nameof(Tutor.Id))]
        public long TutorId { get; set; }
        [EntityBind(nameof(User.ImageName))]
        public string? TutorImage { get; set; }
        [EntityBind(nameof(User.Name))]
        public string TutorName { get; set; }

        [EntityBind(nameof(User.PaymentExists), nameof(User.BuyerPayment.PaymentKeyExpiration))]
        public bool NeedPayment { get; set; }

        public CouponType? CouponType { get; set; }

        public bool ShouldSerializeCouponType() => false;
        public bool ShouldSerializeCouponValue() => false;

        public decimal? CouponValue { get; set; }

        [EntityBind(nameof(StudyRoom.Price), nameof(Tutor.Price))]
        public decimal TutorPrice { get; set; }
        public string Jwt { get; set; }

        [EntityBind(nameof(StudyRoom.BroadcastTime))]
        public DateTime? BroadcastTime { get; set; }

        [EntityBind(nameof(StudyRoom.Name))]
        public string Name { get; set; }


        public StudyRoomType Type { get; set; }
    };



    public class UserStudyRoomDto
    {
        public string Name { get; set; }
        //public string? Image { get; set; }
        //public long UserId { get; set; }
        //public bool Online { get; set; }
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }

        public string ConversationId { get; set; }
        public DateTime? LastSession { get; set; }

        public StudyRoomType Type { get; set; }

    }
}