using System;

namespace Zbang.Zbox.Domain
{
    public class FlashcardPin
    {
        protected FlashcardPin()
        {

        }

        public FlashcardPin(Guid id, User user, FlashcardMeta flashcard,int index) :this()
        {
            Id = id;
            User = user;
            Flashcard = flashcard;
            Index = index;
            DateTime = DateTime.UtcNow;
        }

        public Guid Id { get; set; }
        public int Index { get; set; }

        public User User { get; set; }

        public FlashcardMeta Flashcard { get; set; }

        public DateTime DateTime { get; set; }
    }
}