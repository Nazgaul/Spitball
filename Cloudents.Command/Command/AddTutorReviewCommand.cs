using System;

namespace Cloudents.Command.Command
{
    public class AddTutorReviewCommand : ICommand
    {
        public AddTutorReviewCommand(Guid roomId, string review, float rate, long tutorId, long userId)
        {
            RoomId = roomId;
            Review = review;
            Rate = rate;
            TutorId = tutorId;
            UserId = userId;
        }
        public virtual Guid RoomId { get; set; }
        public virtual string Review { get; }
        public virtual float Rate { get; }
        public long TutorId { get;  }
        public long UserId { get; }
    }
}