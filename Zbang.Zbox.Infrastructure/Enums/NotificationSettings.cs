
using Zbang.Zbox.Infrastructure.Enums.Resources;
using Zbang.Zbox.Infrastructure.Extensions;
namespace Zbang.Zbox.Infrastructure.Enums
{
    //if you add another value you should update views
    public enum NotificationSettings
    {
        [EnumDescription(typeof(EnumResources), "NotificationSettingsOff")]
        Off = 1,
        [EnumDescription(typeof(EnumResources), "NotificationSettingsOnEveryChange")]
        OnEveryChange = 2,
        [EnumDescription(typeof(EnumResources), "NotificationSettingsOnceADay")]
        OnceADay = 24, // hour in a day
        [EnumDescription(typeof(EnumResources), "NotificationSettingsOnceAWeek")]
        OnceAWeek = 168 // hour in a week
    }
}
