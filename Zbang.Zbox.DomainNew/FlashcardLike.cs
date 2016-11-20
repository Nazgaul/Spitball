using System;

namespace Zbang.Zbox.Domain
{
    public class FlashcardLike
    {
        protected FlashcardLike()
        {

        }

        public FlashcardLike(Guid id, User user, FlashcardMeta flashcard) : this()
        {
            Id = id;
            User = user;
            Flashcard = flashcard;
           
            DateTime = DateTime.UtcNow;
        }
        public Guid Id { get; set; }

        public virtual User User { get; set; }

        public virtual FlashcardMeta Flashcard { get; set; }

        public DateTime DateTime { get; set; }

    }

    public class FlashcardSolve
    {
        protected FlashcardSolve()
        {

        }

        public FlashcardSolve(Guid id, User user, FlashcardMeta flashcard) : this()
        {
            Id = id;
            User = user;
            Flashcard = flashcard;

            DateTime = DateTime.UtcNow;
        }
        public Guid Id { get; set; }

        public virtual User User { get; set; }

        public virtual FlashcardMeta Flashcard { get; set; }

        public DateTime DateTime { get; set; }

    }
}