using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.DTOs.Admin
{
    public class PaymentDto
    {
        public Guid StudyRoomSessionId { get; set; }
        public decimal Price { get; set; }
        [EntityBind(nameof(Tutor.SellerKey))]
        public string SellerKey { get; set; }
        [EntityBind(nameof(User.BuyerPayment.PaymentKey))]
        public string PaymentKey { get; set; }
        [EntityBind(nameof(Tutor.Id))]
        public long TutorId { get; set; }
        [EntityBind(nameof(User.Name))]
        public string TutorName { get; set; }
        [EntityBind(nameof(User.Id))]
        public long UserId { get; set; }
        [EntityBind(nameof(User.Name))]
        public string UserName { get; set; }
    }
}
