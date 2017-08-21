using System.Collections;
using System.Collections.Generic;
using System.Web.ModelBinding;

namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class Flashcard
    {
        [BindNever]
        public long? Id { get; set; }
        public string Name { get; set; }

        public IList<Card> Cards { get; set; }
    }

    public class Card
    {
        public Slide Front { get; set; }
        public Slide Cover { get; set; }

        public bool IsEmpty()
        {
            //var sx = Front?.IsEmpty();
            //sx.GetValueOrDefault()
            if (Front != null && !Front.IsEmpty())
            {
                return false;
            }
            if (Cover != null && !Cover.IsEmpty())
            {
                return false;
            }
            return true;
        }
    }

    public class Slide
    {
        public string Text { get; set; }
        public string Image { get; set; }

        public bool IsEmpty()
        {
            return string.IsNullOrWhiteSpace(Text) && string.IsNullOrEmpty(Image);
        }
    }
}