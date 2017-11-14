using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs
{
    public class SiteMapCountDto
    {
        public SiteMapCountDto(SeoType type, int count)
        {
            Type = type;
            Count = count;
        }

        public SeoType Type { get; }
        public int Count { get; }
    }
}
