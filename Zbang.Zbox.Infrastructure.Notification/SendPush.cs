using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Notifications;
using Zbang.Zbox.Infrastructure.Notification;

namespace Zbang.Zbox.Infrastructure.Notifications
{
    public class SendPush
    {
        readonly NotificationHubClient m_Hub = NotificationHubClient.CreateClientFromConnectionString("<connection string with full access>", "<hub name>");
        public SendPush()
        {

        }
        private Task SendNotificationAsync(IPushMessage message, IEnumerable<string> tags)
        {

            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            if (tags == null)
            {
                throw new ArgumentNullException("tags");
            }
            var notification = CreateNotification(message);
            return m_Hub.SendNotificationAsync(notification, tags);
        }

        private static Microsoft.ServiceBus.Notifications.Notification CreateNotification(IPushMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            WindowsPushMessage windowsPushMessage;
            Microsoft.ServiceBus.Notifications.Notification result;
                        if (message is GooglePushMessage)
                        {
                            result = new GcmNotification(message.ToString());
                        }
                        
                    }
                }
            }
            return result;
        }
    }
    public interface IPushMessage
    {
    }
}
