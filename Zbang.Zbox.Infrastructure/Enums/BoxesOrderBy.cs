using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Enums.Resources;

namespace Zbang.Zbox.Infrastructure.Enums
{
    public enum OrderBy
    {
        [EnumDescription(typeof(EnumResources), "BoxesOrderLastModified")]
        LastModified = 0,
        [EnumDescription(typeof(EnumResources), "BoxesOrderName")]
        Name = 1
    }
}
