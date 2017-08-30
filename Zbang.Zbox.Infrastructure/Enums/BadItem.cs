using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Enums.Resources;

namespace Zbang.Zbox.Infrastructure.Enums
{
    public enum BadItem
    {
        [ResourceDescription(typeof(EnumResources), "Infringe")]
        RightViolation = 1,
        [ResourceDescription(typeof(EnumResources), "Spam")]
        Spam,
        [ResourceDescription(typeof(EnumResources), "Nudity")]
        Porn,
        [ResourceDescription(typeof(EnumResources), "Attacks")]
        Attack,
        [ResourceDescription(typeof(EnumResources), "Hateful")]
        Hate,
        [ResourceDescription(typeof(EnumResources), "Other")]
        Other,
        [ResourceDescription(typeof(EnumResources), "CannotView")]
        CannotView
    }

    //public enum DirtyState
    //{
    //    None = 0,
    //    New = 1,
    //    Delete = 2,
    //    Update = 3
    //}
}
