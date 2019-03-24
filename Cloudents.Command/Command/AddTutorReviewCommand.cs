using Cloudents.Core.Entities;
using System;

namespace Cloudents.Command.Command
{
    public class AddTutorReviewCommand : ICommand
    {
        public AddTutorReviewCommand(string review, float rate, long tutor, RegularUser user)
        {
            Review = review;
            Rate = rate;
            Tutor = tutor;
            User = user;
        }
        public virtual string Review { get; set; }
        public virtual float Rate { get; set; }
        public long Tutor { get; set; }
        public RegularUser User { get; set; }
    }
}