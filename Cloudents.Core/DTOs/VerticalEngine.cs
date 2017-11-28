using System.Collections.Generic;
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs
{
    public abstract class VerticalEngineDto
    {
        protected VerticalEngineDto(IEnumerable<string> term)
        {
            Term = term;
        }


        public abstract Vertical Vertical { get; }
        public IEnumerable<string> Term { get; }

    }


    public class NoneEngineDto : VerticalEngineDto
    {
        public NoneEngineDto() : base(null)
        {

        }
        public override Vertical Vertical => Vertical.None;
    }

    public class DocumentEngineDto : VerticalEngineDto
    {
        public DocumentEngineDto(IEnumerable<string> term, string university) : base(term)
        {
            University = university;
        }

        public override Vertical Vertical => Vertical.Document;

        public string University { get; private set; }
    }

    public class AskEngineDto : VerticalEngineDto
    {
        public AskEngineDto(IEnumerable<string> term) : base(term)
        {
        }

        public override Vertical Vertical => Vertical.Ask;
    }

    public class JobEngineDto : VerticalEngineDto
    {
        public JobEngineDto(IEnumerable<string> term, string location) : base(term)
        {
            Location = location;
        }


        public override Vertical Vertical => Vertical.Job;

        public string Location { get; private set; }
    }

    public class TutorEngineDto : VerticalEngineDto
    {
        public TutorEngineDto(IEnumerable<string> term, string location) : base(term)
        {
            Location = location;
        }

        public override Vertical Vertical => Vertical.Tutor;

        public string Location { get; private set; }
    }

    public class FoodEngineDto : VerticalEngineDto
    {
        public FoodEngineDto(IEnumerable<string> term, string location) : base(term)
        {
            Location = location;
        }
        public override Vertical Vertical => Vertical.Food;

        public string Location { get; private set; }
    }

    public class BookEngineDto : VerticalEngineDto
    {
        public BookEngineDto(IEnumerable<string> term) : base(term)
        {
        }

        public override Vertical Vertical => Vertical.Book;
    }



    public class FlashcardEngineDto : VerticalEngineDto
    {
        public FlashcardEngineDto(IEnumerable<string> term, string university) : base(term)
        {
            University = university;
        }

        public override Vertical Vertical => Vertical.Flashcard;

        public string University { get; }
    }
}
