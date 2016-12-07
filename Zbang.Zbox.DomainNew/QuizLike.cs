using System;

namespace Zbang.Zbox.Domain
{
    public class QuizLike
    {
        protected QuizLike()
        {

        }

        public QuizLike(Guid id, User user, Quiz quiz) : this()
        {
            Id = id;
            User = user;
            Quiz = quiz;

            DateTime = DateTime.UtcNow;
        }
        public Guid Id { get; set; }

        public virtual User User { get; set; }

        public virtual Quiz Quiz { get; set; }

        public DateTime DateTime { get; set; }

    }
}