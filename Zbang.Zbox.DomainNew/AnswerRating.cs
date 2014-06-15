using System;

namespace Zbang.Zbox.Domain
{
    public class AnswerRating
    {
        protected AnswerRating()
        {

        }
        public AnswerRating(Guid id, User user, CommentReplies answer)
        {
// ReSharper disable DoNotCallOverridableMethodsInConstructor
            Id = id;
            User = user;
            Answer = answer;
            RateUp = true;
            IncrementCount();
            // ReSharper restore DoNotCallOverridableMethodsInConstructor

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
