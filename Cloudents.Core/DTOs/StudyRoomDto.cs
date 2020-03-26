using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using System;
using System.Diagnostics.CodeAnalysis;

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
        [EntityBind(nameof(User.Id))]
        public long StudentId { get; set; }
        [EntityBind(nameof(User.ImageName))]
        public string StudentImage { get; set; }
        [EntityBind(nameof(User.Name))]
        public string StudentName { get; set; }


        [EntityBind(nameof(User.PaymentExists), nameof(User.BuyerPayment.PaymentKeyExpiration))]
        public bool NeedPayment { get; set; }

        public CouponType? CouponType { get; set; }

        public bool ShouldSerializeCouponType() => false;
        public bool ShouldSerializeCouponValue() => false;

        public decimal? CouponValue { get; set; }

        public decimal TutorPrice { get; set; }
        public string Jwt { get; set; }
    };



    public class UserStudyRoomDto
    {
        public string Name { get; set; }
        public string? Image { get; set; }
        public long UserId { get; set; }
        public bool Online { get; set; }
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }

        public string ConversationId { get; set; }
        public DateTime LastSession { get; set; }

    }
}