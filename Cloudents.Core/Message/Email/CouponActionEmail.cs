using Cloudents.Core.Message.System;

namespace Cloudents.Core.Message.Email
{
    public class CouponActionEmail : ISystemQueueMessage
    {
        public CouponActionEmail(long id, string? phoneNumber, string email, string couponCode, string tutorName, string couponAction)
        {
            Id = id;
            PhoneNumber = phoneNumber;
            Email = email;
            CouponCode = couponCode;
            TutorName = tutorName;
            CouponAction = couponAction;
        }
        public long Id { get; private set; }
        public string? PhoneNumber { get; private set; }
        public string Email { get; private set; }
        public string CouponCode { get; private set; }
        public string TutorName { get; private set; }
        public string CouponAction { get; private set; }
    }
}
