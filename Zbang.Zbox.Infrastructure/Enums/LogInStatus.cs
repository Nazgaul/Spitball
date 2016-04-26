using Zbang.Zbox.Infrastructure.Enums.Resources;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Zbox.Infrastructure.Enums
{
    public enum LogInStatus
    {
        [EnumDescriptionAttribute(typeof(EnumResources), "UserNotExists")]
        UserNotExists = 1,
        [EnumDescriptionAttribute(typeof(EnumResources), "WrongPassword")]
        WrongPassword,
        [EnumDescriptionAttribute(typeof(EnumResources), "LockedOut")]
        LockedOut,
        Success
    }
}
