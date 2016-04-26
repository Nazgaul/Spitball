﻿using Zbang.Zbox.Infrastructure.Enums.Resources;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Zbox.Infrastructure.Enums
{
    //https://en.wikipedia.org/wiki/ISO/IEC_5218
    public enum Sex : int 
    {
        NotKnown = 0,
        [EnumDescriptionAttribute(typeof(EnumResources), "Male")]
        Male = 1,
        [EnumDescriptionAttribute(typeof(EnumResources), "Female")]
        Female = 2,
        NotApplicable = 9
    }
}
