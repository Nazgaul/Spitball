using System.ComponentModel.DataAnnotations;
using Zbang.Cloudents.Mvc4WebRole.Models.Account.Resources;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Account.Settings
{
    public class UserLanguage : IValidatableObject
    {
        public string Language { get; set; }

        public System.Collections.Generic.IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!Zbang.Zbox.Infrastructure.Culture.Languages.CheckIfLanguageIsSupported(Language))
            {
                yield return new ValidationResult(AccountSettingsResources.LanguageNotSupported, new[] { "Language" });
            }
        }
    }
}