using System.Runtime.Serialization;
using Zbang.Zbox.Infrastructure.Commands;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain.Commands
{
    [DataContract]
    public class ChangeNotificationSettingsCommand : ICommand
    {
        public ChangeNotificationSettingsCommand(long boxId, long userId, NotificationSettings newNotificationSettings)
        {
            BoxId = boxId;
            UserId = userId;
            NewNotificationSettings = newNotificationSettings;
        }
        [DataMember]
        public long BoxId { get; set; }

        /// <summary>
        /// User id
        /// </summary>
        [DataMember]
        public long UserId { get; set; }

        [DataMember]
        public  NotificationSettings NewNotificationSettings { get; set; }
    }
}
