using System;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities
{
    public class TutorReview : Entity<Guid>
    {
        public TutorReview(string review, float rate, RegularUser user, RegularUser tutor)
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
        public virtual string Review { get;protected set; }
                
        public virtual float Rate { get; protected set; }
                
        public virtual DateTime DateTime { get; protected set; }
                
        public virtual RegularUser User { get; protected set; }
                
        public virtual RegularUser Tutor { get; set; }
    }
}