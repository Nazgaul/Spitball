using System.Collections.Generic;
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs
{
    public class VerticalEngineSearchDto : VerticalEngineDto
    {
        public VerticalEngineSearchDto(IEnumerable<string> term, string university) : base(term)
        {
            University = university;
        }

        public override Vertical Vertical => Vertical.Document;

        public string University { get; }
    }
}