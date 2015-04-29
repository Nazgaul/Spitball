using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Notifications;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;

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
        private Task SendNotificationAsync(Notification message, IEnumerable<long> tags)
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

            return m_Hub.SendNotificationAsync(message, tags.Select(s => s.ToString(CultureInfo.InvariantCulture)));
        }
       

        private Task SendNotification(GooglePushMessage googleMessage, ApplePushMessage appleMessage, ICollection<long> tags)
        {
            if (tags.Count == 0)
            {
                return Task.FromResult<string>(null);
            }

            var list = new List<Task>();
            for (int i = 0; i <= tags.Count / UsersPerPage; i++)
            {
                var users = tags.Skip(i*UsersPerPage).Take(UsersPerPage).ToList();
                if (googleMessage != null)
                {
                    TraceLog.WriteInfo(String.Format("sending gcm push notification data: {0} to users {1}",
                        googleMessage, String.Join(",", users)));
                    list.Add(SendNotificationAsync(new GcmNotification(googleMessage.ToString()),users));
                }
                if (appleMessage != null)
                {
                    list.Add(SendNotificationAsync( new AppleNotification(appleMessage.ToString()), users));
                }

            }
            return Task.WhenAll(list);
        }

        public Task SendAddPostNotification(string userNameOfAction,
            string text,
            string boxName,
            IList<long> userIds)
        {
            var googleMessage = new GooglePushMessage(
             new Dictionary<string, string>
                {
                    {"boxName", boxName },
                    {"text", text},
                    {"userName", userNameOfAction},
                    {"action",((int)PushAction.PostComment).ToString(CultureInfo.InvariantCulture)}
                }, null);
            var applePushMessage = new ApplePushMessage();
            applePushMessage.Aps.AlertProperties.LocKey = "PUSH_NOTIFICATION_COMMENT";
            applePushMessage.Aps.AlertProperties["loc-args"] = new[]
                {
                    userNameOfAction, boxName,text
                
                };
            return SendNotification(googleMessage, applePushMessage, userIds);
        }

        public Task SendAddReplyNotification(string userNameOfAction,
            string text,
            string boxName,
            IList<long> userIds)
        {
            var googleMessage = new GooglePushMessage(
             new Dictionary<string, string>
                {
                    {"boxName", boxName },
                    {"text", text},
                    {"userName", userNameOfAction},
                    {"action",((int)PushAction.PostReply).ToString(CultureInfo.InvariantCulture)}
                }, null);

            var applePushMessage = new ApplePushMessage();
            applePushMessage.Aps.AlertProperties.LocKey = "PUSH_NOTIFICATION_COMMENT";
            applePushMessage.Aps.AlertProperties["loc-args"] = new[]
                {
                    userNameOfAction, boxName,text
                
                };

            return SendNotification(googleMessage, applePushMessage, userIds);
        }

        public Task SendAddItemNotification(string userNameOfAction,
            string boxName,
            IList<long> userIds)
        {
            var googleMessage = new GooglePushMessage(
             new Dictionary<string, string>
                {
                    {"boxName", boxName },
                    {"userName", userNameOfAction},
                    {"action",((int)PushAction.AddItem).ToString(CultureInfo.InvariantCulture)}
                }, null);
            var applePushMessage = new ApplePushMessage();
            applePushMessage.Aps.AlertProperties.LocKey = "PUSH_NOTIFICATION_FILE";
            applePushMessage.Aps.AlertProperties["loc-args"] = new[]
                {
                    userNameOfAction, boxName
                
                };
            return SendNotification(googleMessage, applePushMessage, userIds);
        }

        public Task SendInviteNotification(string userNameOfAction,
            string boxName,
            long userId)
        {
            var googleMessage = new GooglePushMessage(
             new Dictionary<string, string>
                {
                    {"boxName", boxName },
                   
                    {"userName", userNameOfAction},
                    {"action",((int)PushAction.Invite).ToString(CultureInfo.InvariantCulture)}
                }, null);
            return SendNotification(googleMessage, null, new[] { userId });
        }


        //private static Notification CreateGcmNotification(IPushMessage message)
        //{
        //    if (message == null)
        //    {
        //        throw new ArgumentNullException("message");
        //    }
        //    return new GcmNotification(message.ToString());

        //}

        //private static Notification CreateAppleNotification(IPushMessage message)
        //{
        //    if (message == null)
        //    {
        //        throw new ArgumentNullException("message");
        //    }
        //    return new AppleNotification(message.ToString());

        //}
    }
}
