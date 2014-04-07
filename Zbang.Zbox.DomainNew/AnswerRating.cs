using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Domain
{
    public class AnswerRating
    {
        protected AnswerRating()
        {

        }
        public AnswerRating(Guid id, User user, CommentReplies answer)
        {
            Id = id;
            User = user;
            Answer = answer;
            RateUp = true;
            IncrementCount();

        }

        public void ChangeUserRating()
        {
            RateUp = !RateUp;
            if (RateUp)
            {
                IncrementCount();
            }
            else
            {
                Answer.RatingCount--;
            }
        }

        public void IncrementCount()
        {
            Answer.RatingCount++;
        }

        public virtual Guid Id { get; set; }
        public virtual User User { get; set; }
        public virtual CommentReplies Answer { get; set; }

        public bool RateUp { get; set; }
    }
}
