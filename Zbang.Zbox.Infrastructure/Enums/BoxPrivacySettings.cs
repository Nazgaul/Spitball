using Zbang.Zbox.Infrastructure.Enums.Resources;
using Zbang.Zbox.Infrastructure.Extensions;
namespace Zbang.Zbox.Infrastructure.Enums
{
    //if you add another value you should update views
    public enum BoxPrivacySettings
    {
        [EnumDescription(typeof(EnumResources), "BoxPrivacySettingsMembersOnly")]
        MembersOnly = 2,
        [EnumDescription(typeof(EnumResources), "BoxPrivacySettingsAnyoneWithUrl")]
        AnyoneWithUrl = 3,
        PrivateUniversity = 4

    }

    public enum LibraryNodeSettings
    {
        Open = 0,
        Closed = 1
    }
}