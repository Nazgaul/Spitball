using System.ComponentModel.DataAnnotations;
using Zbang.Cloudents.Mobile.Models.Account.Resources;

namespace Zbang.Cloudents.Mobile.Models.Account.Settings
{
    public class UserLanguage : IValidatableObject
    {
        public string Language { get; set; }

        public System.Collections.Generic.IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!Zbox.Infrastructure.Culture.Languages.CheckIfLanguageIsSupported(Language))
            {
                yield return new ValidationResult(AccountSettingsResources.LanguageNotSupported, new[] { "Language" });
            }
        }
    }
}