using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs
{
    public class VerticalEngineNoneDto : VerticalEngineDto
    {
        public VerticalEngineNoneDto() : base(null)
        {

        }
        public override Vertical Vertical => Vertical.None;
    }
}