using System.ComponentModel.DataAnnotations;
using System.Text;
using Zbang.Zbox.Domain;

namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class UpdateBoxInfo
    {
        [Required]
        public long Id { get; set; }

        [Required]
        [StringLength(Box.NameLength)]
        public string Name { get; set; }

        public string CourseCode { get; set; }

        public string Professor { get; set; }

        public Zbox.Infrastructure.Enums.BoxPrivacySetting? BoxPrivacy { get; set; }

        public Zbox.Infrastructure.Enums.NotificationSetting Notification { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("boxid: {0}", Id));
            sb.AppendLine(string.Format("Name: {0}", Name));
            sb.AppendLine(string.Format("CourseCode: {0}", CourseCode));
            sb.AppendLine(string.Format("Professor: {0}", Professor));
            sb.AppendLine(string.Format("BoxPrivacy: {0}", BoxPrivacy));
            sb.AppendLine(string.Format("Notification: {0}", Notification));
            return base.ToString();
        }
    }
}