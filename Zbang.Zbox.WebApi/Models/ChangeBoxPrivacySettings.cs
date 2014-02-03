using System.ComponentModel.DataAnnotations;

using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.WebApi.Models
{
    public class ChangeBoxPrivacySettings
    {
        [Required]
        public BoxPrivacySettings NewBoxPrivacySettings { get; set; }

        public override string ToString()
        {
            return string.Format(" NewBoxPrivacySettings {0}",
                    NewBoxPrivacySettings);

        }
    }
}