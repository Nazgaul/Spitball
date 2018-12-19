using Cloudents.Application.Enum;

namespace Cloudents.Application.DTOs
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
