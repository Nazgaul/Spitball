using System;

namespace Cloudents.Command.Command
{
    public class AddTutorReviewCommand : ICommand
    {
        public AddTutorReviewCommand(Guid roomId, string? review, float rate, long userId)
        {
            RoomId = roomId;
            Review = review;
            Rate = rate;
            UserId = userId;
        }
        public Guid RoomId { get; }
        public string? Review { get; }
        public float Rate { get; }
        public long UserId { get; }
    }
}