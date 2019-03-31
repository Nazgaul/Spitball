using Cloudents.Core.Entities;

namespace Cloudents.Command.Command
{
    public class AddTutorReviewCommand : ICommand
    {
        public AddTutorReviewCommand(string review, float rate, long tutorId, long userId)
        {
            Review = review;
            Rate = rate;
            TutorId = tutorId;
            UserId = userId;
        }
        public virtual string Review { get; }
        public virtual float Rate { get; }
        public long TutorId { get;  }
        public long UserId { get; }
    }
}