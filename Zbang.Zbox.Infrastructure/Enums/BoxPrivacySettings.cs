using Zbang.Zbox.Infrastructure.Enums.Resources;
using Zbang.Zbox.Infrastructure.Extensions;
namespace Zbang.Zbox.Infrastructure.Enums
{
    //if you add another value you should update views
    public enum BoxPrivacySettings
    {
        //[EnumDescription(typeof(EnumResources), "BoxPrivacySettingsPrivate")]
        //Private = 0,
        [EnumDescription(typeof(EnumResources), "BoxPrivacySettingsMembersOnly")]
        MembersOnly = 2,
        [EnumDescription(typeof(EnumResources), "BoxPrivacySettingsAnyoneWithUrl")]
        AnyoneWithUrl = 3
    }
}