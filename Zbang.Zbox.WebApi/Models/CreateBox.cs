using System.ComponentModel.DataAnnotations;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.WebApi.Models
{
    public class CreateBox
    {
        [Required]
        [StringLength(Zbang.Zbox.Domain.Box.NameLength)]
        //[RegularExpression(Validation.WindowFileRegex)]
        public string BoxName { get; set; }
        [Required]
        public NotificationSettings NotificationSettings { get; set; }
        [Required]
        public BoxPrivacySettings BoxPrivacySettings { get; set; }

        public override string ToString()
        {
            return string.Format("  boxName {0} NotificationSettings {1} BoxPrivacySettings {2}",
                     BoxName, NotificationSettings, BoxPrivacySettings);

        }

    }
}