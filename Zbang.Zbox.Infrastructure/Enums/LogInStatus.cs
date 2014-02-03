using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Infrastructure.Enums.Resources;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Zbox.Infrastructure.Enums
{
    public enum LogInStatus
    {
        [EnumDescription(typeof(EnumResources), "UserNotExists")]
        UserNotExists = 1,
        [EnumDescription(typeof(EnumResources), "WrongPassword")]
        WrongPassword,
        [EnumDescription(typeof(EnumResources), "LockedOut")]
        LockedOut,
        Success
    }
}
