using System.ComponentModel.DataAnnotations;
using Zbang.Cloudents.Mvc4WebRole.Views.Box.Resources;
using Zbang.Cloudents.Mvc4WebRole.Views.Shared.Resources;
using Zbang.Zbox.Domain;

namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class UpdateBoxInfo
    {
        [Required]
        public long BoxUid { get; set; }

        [Required]
        [StringLength(Box.NameLength)]
        [Display(ResourceType = typeof(SharedResources), Name = "Name")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(IndexResources), Name = "Number")]
        public string CourseCode { get; set; }

        [Display(ResourceType = typeof(IndexResources), Name = "Lecturer")]
        public string Professor { get; set; }

        public Zbox.Infrastructure.Enums.BoxPrivacySettings? BoxPrivacy { get; set; }

        public Zbox.Infrastructure.Enums.NotificationSettings Notification { get; set; }

        public string Picture { get; set; }
    }
}