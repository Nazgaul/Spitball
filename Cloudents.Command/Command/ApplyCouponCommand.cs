using System;

namespace Cloudents.Command.Command
{
    public class ApplyCouponCommand : ICommand
    {
        public ApplyCouponCommand(string coupon, long userId, long tutorId, Guid roomId)
        {
            Coupon = coupon;
            UserId = userId;
            TutorId = tutorId;
            RoomId = roomId;
        }

        public string Coupon { get;  }
        public long UserId { get;  }

        public Guid RoomId { get; }

        public double NewPrice { get; set; }
        public long TutorId { get;  }
    }
}