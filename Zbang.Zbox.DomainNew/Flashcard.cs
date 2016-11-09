using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain
{
    public class Flashcard : IDirty
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool Publish { get; set; }
        public int LikeCount { get; set; }
        public int CardCount { get; set; }

        public bool IsDeleted { get; set; }

        //public ICollection<Card> Cards { get; set; }
        public void DeleteAssociation()
        {
            //throw new NotImplementedException();
        }

        public bool IsDirty { get; set; }
        public Func<bool> ShouldMakeDirty { get; }

        public virtual UserTimeDetails DateTimeUser { get; private set; }
    }

    //public class Card
    //{
    //    public Guid Id { get; set; }
    //    public string Text { get; set; }
    //    public bool Front { get; set; }
    //    public string Image { get; set; }
    //    public int Order { get; set; }

    //    public Flashcard Flashcard { get; set; }
    //}

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
