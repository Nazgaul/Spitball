﻿using Zbang.Zbox.Infrastructure.Commands;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain.Commands
{
    public class ChangeNotificationSettingsCommand : ICommand
    {
        public ChangeNotificationSettingsCommand(long boxId, long userId, NotificationSetting newNotificationSettings)
        {
            BoxId = boxId;
            UserId = userId;
            NewNotificationSettings = newNotificationSettings;
        }
        public long BoxId { get; set; }

        /// <summary>
        /// User id
        /// </summary>
        public long UserId { get; set; }

        public  NotificationSetting NewNotificationSettings { get; set; }
    }
}
