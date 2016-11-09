using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain
{
    [DocumentDbModel("Flashcard")]
    public class Flashcard //: IDirty
    {
        
        public long Id { get; set; }
        public string Name { get; set; }
        public bool Publish { get; set; }
        //public int LikeCount { get; set; }
        //public int CardCount { get; set; }

        //public bool IsDeleted { get; set; }

        //public ICollection<Card> Cards { get; set; }

        //public bool IsDirty { get; set; }
        //public Func<bool> ShouldMakeDirty { get; }


        public IEnumerable<Card> Cards { get; set; }
        public DateTime DateTime { get; set; }
    }

    public class Card
    {
        public CardSlide Front { get; set; }
        public CardSlide Cover { get; set; }
    }

    public class CardSlide
    {
        public string Text { get; set; }
        public string Image { get; set; }


        //public Flashcard Flashcard { get; set; }
    }

    public class PinnedCard
    {
        public Guid Id { get; set; }
        public User User { get; set; }
        public int Order { get; set; }
        //public Card Card { get; set; }
        public Flashcard Flashcard { get; set; }
    }

    public class FlashcardLike
    {
        public Guid Id { get; set; }
        public User User { get; set; }
        public Flashcard Flashcard { get; set; }
        public DateTime CreationTime { get; protected set; }
    }
}
