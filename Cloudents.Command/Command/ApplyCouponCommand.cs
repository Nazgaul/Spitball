namespace Cloudents.Command.Command
{
    public class ApplyCouponCommand : ICommand
    {
        public ApplyCouponCommand(string coupon, long userId,  long courseId)
        {
            Coupon = coupon;
            UserId = userId;
            CourseId = courseId;
        }

        public string Coupon { get;  }
        public long UserId { get;  }

        public long CourseId { get; }

        public double NewPrice { get; set; }
    }
}