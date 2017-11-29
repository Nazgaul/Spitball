using System.Collections.Generic;
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs
{
    public class VerticalEngineFlashcardDto : VerticalEngineDto
    {
        public VerticalEngineFlashcardDto(IEnumerable<string> term, string university) : base(term)
        {
            University = university;
        }

        public override Vertical Vertical => Vertical.Flashcard;

        public string University { get; }
    }
}