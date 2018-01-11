using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
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
            id = Id.ToString(CultureInfo.InvariantCulture);
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public bool Publish { get; set; }

        public long UserId { get; set; }

        public long BoxId { get; set; }

        // ReSharper disable once InconsistentNaming Need for document db
        [SuppressMessage("", "IDE1006", Justification = "Documentdb id is lowercase")]
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
    }
}
