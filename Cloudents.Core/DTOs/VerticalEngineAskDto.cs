using System.Collections.Generic;
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs
{
    public class VerticalEngineAskDto : VerticalEngineDto
    {
        public VerticalEngineAskDto(IEnumerable<string> term) : base(term)
        {
        }

        public override Vertical Vertical => Vertical.Ask;
    }
}