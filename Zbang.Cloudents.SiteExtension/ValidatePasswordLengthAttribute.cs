using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.SiteExtension
{
    public sealed class ValidatePasswordLengthAttribute : MinLengthAttribute
    {
        //private static string _defaultErrorMessage = ValidatePasswordResources.MustBeAtLeast;
        //private readonly int m_MinCharacters = Membership.Provider.MinRequiredPasswordLength;
        public ValidatePasswordLengthAttribute()
            : base(6)
        {

        }
    }
}