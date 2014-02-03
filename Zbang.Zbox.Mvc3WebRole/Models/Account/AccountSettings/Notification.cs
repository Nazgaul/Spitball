using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Zbang.Zbox.Mvc3WebRole.Models.Account.Resources;

namespace Zbang.Zbox.Mvc3WebRole.Models.Account.AccountSettings
{
    public class Notification
    {
        [Display(ResourceType = typeof(AccountSettingsResources), Name = "Notifications")]
        public Zbang.Zbox.Infrastructure.Enums.NotificationSettings NotificationSettings { get; set; }


        public string BoxUid { get; set; }
    }
}