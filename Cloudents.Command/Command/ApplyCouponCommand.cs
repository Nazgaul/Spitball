namespace Cloudents.Command.Command
{
    public class ApplyCouponCommand : ICommand
    {
        public ApplyCouponCommand(string coupon, long userId, long tutorId)
        {
            Coupon = coupon;
            UserId = userId;
            TutorId = tutorId;
        }

        public string Coupon { get;  }
        public long UserId { get;  }

        public decimal NewPrice { get; set; }
        public long TutorId { get;  }
    }
}