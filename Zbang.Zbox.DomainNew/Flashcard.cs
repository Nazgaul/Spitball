using System;
using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain
{
    [DocumentDbModel("Flashcard")]
    public class Flashcard //: IDirty
    {
        protected Flashcard()
        {
            
        }
        public Flashcard(long identifier)
        {
            Id = identifier;
            id = Id.ToString();
        }
        public long Id { get; set; }
        public string Name { get; set; }
        public bool Publish { get; set; }

        public long UserId { get; set; }

        public long BoxId { get; set; }

        
        // ReSharper disable once InconsistentNaming Need for document db
        public string id { get; set; }

        public bool IsDeleted { get; set; }


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

    //public class PinnedCard
    //{
    //    public Guid Id { get; set; }
    //    public User User { get; set; }
    //    public int Order { get; set; }
    //    //public Card Card { get; set; }
    //    public Flashcard Flashcard { get; set; }
    //}

    //public class FlashcardLike
    //{
    //    public Guid Id { get; set; }
    //    public User User { get; set; }
    //    public Flashcard Flashcard { get; set; }
    //    public DateTime CreationTime { get; protected set; }
    //}
}
