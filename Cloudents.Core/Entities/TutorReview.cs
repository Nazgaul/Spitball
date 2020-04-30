using Cloudents.Core.Event;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate proxy")]
    public class TutorReview : Entity<Guid>
    {
        internal TutorReview(string? review, float rate, User user, Tutor tutor/*, StudyRoom room*/)
        {
            if (rate <= 0) throw new ArgumentOutOfRangeException(nameof(rate));
            if (rate > 5) throw new ArgumentOutOfRangeException(nameof(rate));
            if (!string.IsNullOrEmpty(review))
            {
                Review = review;
            }
            Rate = rate;
            User = user;
            Tutor = tutor;
            DateTime = DateTime.UtcNow;

            //on tutor it will work but the commit will come after so the review count will fucked up
            AddEvent(new TutorAddReviewEvent(tutor.Id));

        }

        [SuppressMessage("ReSharper", "CS8618", Justification = "Nhibernate proxy")]
        [SuppressMessage("ReSharper", "UnusedMember.Global",  Justification = "Nhibernate proxy")]
        protected TutorReview()
        {

        }
        public virtual string? Review { get; protected set; }

        public virtual float Rate { get; protected set; }

        public virtual DateTime DateTime { get; protected set; }

        public virtual User User { get; protected set; }

        public virtual Tutor Tutor { get; protected set; }
        //public virtual StudyRoom Room { get; protected set; }

        public virtual byte[] Version { get; protected set; }

        public virtual bool IsShownHomePage { get; protected set; }

    }
}