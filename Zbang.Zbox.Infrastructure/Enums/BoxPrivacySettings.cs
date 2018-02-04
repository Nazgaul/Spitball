using Zbang.Zbox.Infrastructure.Enums.Resources;
using Zbang.Zbox.Infrastructure.Extensions;
namespace Zbang.Zbox.Infrastructure.Enums
{
    //if you add another value you should update views
    public enum BoxPrivacySetting
    {
        [ResourceDescription(typeof(EnumResources), "BoxPrivacySettingsMembersOnly")]
        MembersOnly = 2,
        [ResourceDescription(typeof(EnumResources), "BoxPrivacySettingsAnyoneWithUrl")]
        AnyoneWithUrl = 3,
    }
}