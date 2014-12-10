using System.ComponentModel.DataAnnotations;
using Zbang.Cloudents.Mobile.Models.Account.Resources;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Account.Settings
{
    public class Notification
    {
        [Display(ResourceType = typeof(AccountSettingsResources), Name = "Notifications")]
        public Zbox.Infrastructure.Enums.NotificationSettings NotificationSettings { get; set; }


        public string BoxUid { get; set; }
    }
}