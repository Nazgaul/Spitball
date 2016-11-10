using System;
using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain
{
    public class FlashCardMeta : IDirty
    {
        protected FlashCardMeta()
        {
            ShouldMakeDirty = () => true;
        }

        public FlashCardMeta(long id, string name, User user, Box box) : this()
        {
            Id = id;
            Name = name?.Trim();
            User = user;
            Box = box;
            DateTimeUser = new UserTimeDetails(user.Id);
        }
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public bool Publish { get; set; }

        public virtual User User { get; set; }
        public int LikeCount { get; set; }
        public int NumberOfViews { get; set; }

        public int CardCount { get; set; }

        public bool IsDeleted { get; set; }
        public void DeleteAssociation()
        {
            //throw new NotImplementedException();
        }


        public bool IsDirty { get; set; }
        public Func<bool> ShouldMakeDirty { get; }

        public virtual UserTimeDetails DateTimeUser { get; private set; }

        public virtual Box Box { get; set; }

    }
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

        
        public string id { get; set; }
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
