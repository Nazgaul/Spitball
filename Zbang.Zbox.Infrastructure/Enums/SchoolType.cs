using Zbang.Zbox.Infrastructure.Enums.Resources;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Zbox.Infrastructure.Enums
{
    public enum SchoolType
    {

        [EnumDescription(typeof(EnumResources), "University")]
        University = 1,
        [EnumDescription(typeof(EnumResources), "College")]
        College = 2,
        [EnumDescription(typeof(EnumResources), "HighSchool")]
        HighSchool = 3
    }
}
