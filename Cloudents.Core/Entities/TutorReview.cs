using System;

namespace Cloudents.Core.Entities
{
    public class TutorReview : Entity<Guid>
    {
        public TutorReview(string review, float rate, RegularUser user, Tutor tutor)
        {
            Review = review;
            Rate = rate;
            User = user;
            Tutor = tutor;
            DateTime = DateTime.UtcNow;
        }

        protected TutorReview()
        {
            
        }
        public virtual string Review { get; set; }
                
        public virtual float Rate { get; set; }
                
        public virtual DateTime DateTime { get; set; }
                
        public virtual RegularUser User { get; set; }
                
        public virtual Tutor Tutor { get; set; }
    }
}