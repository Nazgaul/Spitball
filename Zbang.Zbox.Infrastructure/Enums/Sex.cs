using Zbang.Zbox.Infrastructure.Enums.Resources;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Zbox.Infrastructure.Enums
{
    //https://en.wikipedia.org/wiki/ISO/IEC_5218
    public enum Sex : int 
    {
        NotKnown = 0,
        [ResourceDescription(typeof(EnumResources), "Male")]
        Male = 1,
        [ResourceDescription(typeof(EnumResources), "Female")]
        Female = 2,
        NotApplicable = 9
    }
}
