using System.ComponentModel.DataAnnotations;
using System.Text;
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

        public Zbox.Infrastructure.Enums.BoxPrivacySettings BoxPrivacy { get; set; }

        public Zbox.Infrastructure.Enums.NotificationSettings Notification { get; set; }

        public string Picture { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("boxid: {0}", BoxUid));
            sb.AppendLine(string.Format("Name: {0}", Name));
            sb.AppendLine(string.Format("CourseCode: {0}", CourseCode));
            sb.AppendLine(string.Format("Professor: {0}", Professor));
            sb.AppendLine(string.Format("BoxPrivacy: {0}", BoxPrivacy));
            sb.AppendLine(string.Format("Notification: {0}", Notification));
            sb.AppendLine(string.Format("Picture: {0}", Picture));
            return base.ToString();
        }
    }
}