using Cloudents.Core.Message.System;

namespace Cloudents.Core.Message.Email
{
    public class CouponActionEmail : ISystemQueueMessage
    {
        public CouponActionEmail(long id, string phoneNumber, string email, string couponCode, string tutorName, string couponAction)
        {
            Id = id;
            PhoneNumber = phoneNumber;
            Email = email;
            CouponCode = couponCode;
            TutorName = tutorName;
            CouponAction = couponAction;
        }
        public long Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string CouponCode { get; set; }
        public string TutorName { get; set; }
        public string CouponAction { get; set; }
    }
}
