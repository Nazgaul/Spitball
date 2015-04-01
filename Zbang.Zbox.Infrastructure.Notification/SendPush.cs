using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Notifications;

namespace Zbang.Zbox.Infrastructure.Notifications
{
    public class SendPush : ISendPush
    {
        private readonly NotificationHubClient m_Hub;
        private const int UsersPerPage = 20;

        public SendPush(string connectionString, string hubName)
        {
            if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(hubName))
            {
                return;
            }
            m_Hub = NotificationHubClient.CreateClientFromConnectionString(connectionString, hubName);
        }
        private Task SendNotificationAsync(IPushMessage message, IEnumerable<long> tags)
        {

            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            if (tags == null)
            {
                throw new ArgumentNullException("tags");
            }
            if (m_Hub == null)
            {
                return Task.FromResult(false);
            }
            var notification = CreateNotification(message);

            return m_Hub.SendNotificationAsync(notification, tags.Select(s => s.ToString(CultureInfo.InvariantCulture)));
        }

        private Task SendNotification(PushAction action,
            string userNameOfAction,
            string text,
            string boxName,
            ICollection<long> tags
            )
        {
            if (tags.Count == 0)
            {
                return Task.FromResult<string>(null);
            }
            var message = new GooglePushMessage(
             new Dictionary<string, string>
            {
                {"boxName", boxName },
                {"text",text},
                {"userName", userNameOfAction},
                {"action",((int)action).ToString(CultureInfo.InvariantCulture)}
            }, null);

            var list = new List<Task>();
            for (int i = 0; i <= tags.Count / UsersPerPage; i++)
            {
                list.Add(SendNotificationAsync(message,
                    tags.Skip(i * UsersPerPage).Take(UsersPerPage)));

            }
            return Task.WhenAll(list);
        }

        public Task SendAddPostNotification(string userNameOfAction,
            string text,
            string boxName,
            IList<long> userIds)
        {
            return SendNotification(PushAction.PostComment, userNameOfAction, text, boxName, userIds);
        }

        public Task SendAddReplyNotification(string userNameOfAction,
            string text,
            string boxName,
            IList<long> userIds)
        {
            return SendNotification(PushAction.PostReply, userNameOfAction, text, boxName, userIds);
        }

        public Task SendAddItemNotification(string userNameOfAction,
            string boxName,
            IList<long> userIds)
        {
            return SendNotification(PushAction.AddItem, userNameOfAction, null, boxName, userIds);
        }


        private static Notification CreateNotification(IPushMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            return new GcmNotification(message.ToString());

        }
    }
}
