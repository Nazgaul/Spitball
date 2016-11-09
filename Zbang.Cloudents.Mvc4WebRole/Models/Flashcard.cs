using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;

namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class Flashcard
    {
        [BindNever]
        public long? Id { get; set; }
        public string Name { get; set; }

        public IEnumerable<Card> Cards { get; set; }
    }

    public class Card
    {
        public Slide Front { get; set; }
        public Slide Cover { get; set; }
    }

    public class Slide
    {
        public string Text { get; set; }
        public string Image { get; set; }
    }
}