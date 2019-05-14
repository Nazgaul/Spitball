using System;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate proxy")]
    public class TutorReview : Entity<Guid>
    {
        internal TutorReview(string review, float rate, RegularUser user, Tutor tutor, StudyRoom room)
        {
            if (rate <= 0) throw new ArgumentOutOfRangeException(nameof(rate));
            if (!string.IsNullOrEmpty(review))
            {
                Review = review;
            }

            Rate = rate;
            User = user;
            Tutor = tutor;
            Room = room;
            DateTime = DateTime.UtcNow;
        }

        protected TutorReview()
        {
            
        }
        public virtual string Review { get;protected set; }
                
        public virtual float Rate { get; protected set; }
                
        public virtual DateTime DateTime { get; protected set; }
                
        public virtual RegularUser User { get; protected set; }
                
        public virtual Tutor Tutor { get; protected set; }
        public virtual StudyRoom Room { get; protected set; }
    }
}