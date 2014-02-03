using System.ComponentModel.DataAnnotations;
using Zbang.Zbox.Domain;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Mvc3WebRole.Models.Resources;

namespace Zbang.Zbox.Mvc3WebRole.Models
{
    public class BoxSetting
    {
        [Required]
        public string Uid { get; set; }

        [Display(ResourceType = typeof(BoxSettingResource), Name = "Privacy")]
        public BoxPrivacySettings? Privacy { get; set; }

        [Required(ErrorMessageResourceType = typeof(BoxSettingResource), ErrorMessageResourceName="NameRequired")]
        [StringLength(Box.NameLength, ErrorMessageResourceType = typeof(BoxSettingResource), ErrorMessageResourceName="BoxNameUpTo")]
        [RegularExpression(Validation.WindowFileRegex, ErrorMessageResourceType = typeof(BoxSettingResource), ErrorMessageResourceName = "BoxNameInvalidChar")]
        public string Name { get; set; }
        
        [Required]
        [Display(ResourceType = typeof(BoxSettingResource), Name = "Notifications")]
        public NotificationSettings Notification { get; set; }
    }
}