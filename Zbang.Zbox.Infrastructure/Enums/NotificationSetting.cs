
using Zbang.Zbox.Infrastructure.Enums.Resources;
using Zbang.Zbox.Infrastructure.Extensions;
namespace Zbang.Zbox.Infrastructure.Enums
{
    //if you add another value you should update views
    public enum NotificationSetting
    {
        [ResourceDescription(typeof(EnumResources), "NotificationSettingsOff")]
        Off = 1,
        //[EnumDescription(typeof(EnumResources), "NotificationSettingsOnEveryChange")]
        //OnEveryChange = 2,
        [ResourceDescription(typeof(EnumResources), "NotificationSettingsOnceADay")]
        OnceADay = 24, // hour in a day
        [ResourceDescription(typeof(EnumResources), "NotificationSettingsOnceAWeek")]
        OnceAWeek = 168 // hour in a week
    }
}
