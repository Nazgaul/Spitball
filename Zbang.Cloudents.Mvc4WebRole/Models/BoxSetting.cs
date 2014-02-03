using System.ComponentModel.DataAnnotations;
using Zbang.Cloudents.Mvc4WebRole.Models.Resources;
using Zbang.Zbox.Domain;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class BoxSetting
    {
        [Required]
        public string Uid { get; set; }

        [Required]
        public UserRelationshipType UserType { get; set; }

        [Display(ResourceType = typeof(BoxSettingResource), Name = "Privacy")]
        public BoxPrivacySettings? Privacy { get; set; }

        [Required(ErrorMessageResourceType = typeof(BoxSettingResource), ErrorMessageResourceName="NameRequired")]
        [StringLength(Box.NameLength, ErrorMessageResourceType = typeof(BoxSettingResource), ErrorMessageResourceName="BoxNameUpTo")]
        //[RegularExpression(Validation.WindowFileRegex, ErrorMessageResourceType = typeof(BoxSettingResource), ErrorMessageResourceName = "BoxNameInvalidChar")]
        public string Name { get; set; }
        
        [Required]
        [UIHint("Enum")]
        [Display(ResourceType = typeof(BoxSettingResource), Name = "Notifications")]
        public NotificationSettings Notification { get; set; }
    }
}