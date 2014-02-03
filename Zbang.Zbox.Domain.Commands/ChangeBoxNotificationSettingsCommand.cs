using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Infrastructure.Commands;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain.Commands
{
    public class ChangeBoxNotificationSettingsCommand : ICommand
    {
        public ChangeBoxNotificationSettingsCommand(long boxid,long userId, NotificationSettings notificationSetting)
        {
            NotificationSetting = notificationSetting;
            BoxId = boxid;
            UserId = userId;
        }
        public long BoxId { get; private set; }
        public long UserId { get;private set; }
        public NotificationSettings NotificationSetting { get; private set; }
    }
}
