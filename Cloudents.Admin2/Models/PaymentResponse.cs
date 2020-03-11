using System;

namespace Cloudents.Admin2.Models
{
    public class PaymentResponse
    {
        public Guid StudyRoomSessionId { get; set; }
        public decimal Price { get; set; }

        public bool IsSellerKeyExists { get; set; }
        public bool IsPaymentKeyExists { get; set; }
        public bool IsRealDurationExitsts { get; set; }
        public long TutorId { get; set; }
        public string TutorName { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public DateTime Created { get; set; }
     
        public double Duration { get; set; }
        public double RealDuration { get; set; }
    }
}
