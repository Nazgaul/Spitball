using System.Collections.Generic;
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs
{
    public class VerticalEngineBookDto : VerticalEngineDto
    {
        public VerticalEngineBookDto(IEnumerable<string> term) : base(term)
        {
        }

        public override Vertical Vertical => Vertical.Book;
    }
}