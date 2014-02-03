using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Zbang.Zbox.Mvc3WebRole.Models.Account.Resources;

namespace Zbang.Zbox.Mvc3WebRole.Models.Account.AccountSettings
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