using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Notifications;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Storage;

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
                throw new ArgumentNullException(nameof(message));
            }
            if (tags == null)
            {
                throw new ArgumentNullException(nameof(tags));
            }
            if (m_Hub == null)
            {
                return Task.CompletedTask;
                //return Task.FromResult(false);
            }

            if (ConfigFetcher.IsEmulated)
            {
                return Task.CompletedTask;
                //return Task.FromResult(false);
            }
            return m_Hub.SendNotificationAsync(message, tags.Select(s => s.ToString(CultureInfo.InvariantCulture)));
        }

        private Task SendNotificationAsync(GooglePushMessage googleMessage, ApplePushMessage appleMessage, ICollection<long> tags)
        {
            if (tags.Count == 0)
            {
                return Task.FromResult<string>(null);
            }

            var list = new List<Task>();
            for (int i = 0; i <= tags.Count / UsersPerPage; i++)
            {
                var users = tags.Skip(i * UsersPerPage).Take(UsersPerPage).ToList();
                if (users.Count == 0)
                {
                    continue;
                }
                if (googleMessage != null)
                {
                    //TraceLog.WriteInfo(String.Format("sending gcm push notification data: {0} to users {1}",
                    //    googleMessage, String.Join(",", users)));
                    list.Add(SendNotificationAsync(new GcmNotification(googleMessage.ToString()), users));
                }
                if (appleMessage != null)
                {
                    list.Add(SendNotificationAsync(new AppleNotification(appleMessage.ToString()), users));
                }

            }
            return Task.WhenAll(list);
        }

        public Task SendAddPostNotificationAsync(string userNameOfAction,
            string text,
            string boxName, long boxId,
            IList<long> userIds)
        {
            var googleMessage = new GooglePushMessage(
             new Dictionary<string, string>
                {
                    {"boxName", boxName },
                    {"text", text},
                    {"userName", userNameOfAction},
                    {"action",((int)PushAction.PostComment).ToString(CultureInfo.InvariantCulture)},
                    {"boxId", boxId.ToString(CultureInfo.InvariantCulture)}
                }, null);
            var applePushMessage = new ApplePushMessage();
            applePushMessage.Aps.AlertProperties.LocKey = "PUSH_NOTIFICATION_COMMENT";
            applePushMessage.Aps.AlertProperties["loc-args"] = new[]
                {
                    userNameOfAction, boxName,text
                
                };
            applePushMessage.Add("action", PushAction.PostComment);
            applePushMessage.Add("boxId", boxId);
            return SendNotificationAsync(googleMessage, applePushMessage, userIds);
        }

        public Task SendAddReplyNotificationAsync(string userNameOfAction,
            string text,
            string boxName, long boxId, Guid commentId,
            IList<long> userIds)
        {
            var googleMessage = new GooglePushMessage(
             new Dictionary<string, string>
                {
                    {"boxName", boxName },
                    {"text", text},
                    {"userName", userNameOfAction},
                    {"action",((int)PushAction.PostReply).ToString(CultureInfo.InvariantCulture)},
                    {"boxId", boxId.ToString(CultureInfo.InvariantCulture)},
                    {"commentId", commentId.ToString()}
                }, null);

            var applePushMessage = new ApplePushMessage();
            applePushMessage.Aps.AlertProperties.LocKey = "PUSH_NOTIFICATION_COMMENT";
            applePushMessage.Aps.AlertProperties["loc-args"] = new[]
                {
                    userNameOfAction, boxName,text
                
                };
            applePushMessage.Add("action", PushAction.PostReply);
            applePushMessage.Add("boxId", boxId);
            applePushMessage.Add("commentId", commentId);
            return SendNotificationAsync(googleMessage, applePushMessage, userIds);
        }

        public Task SendAddItemNotificationAsync(string userNameOfAction,
            string boxName, long boxId,
            IList<long> userIds)
        {
            var googleMessage = new GooglePushMessage(
             new Dictionary<string, string>
                {
                    {"boxName", boxName },
                    {"userName", userNameOfAction},
                    {"action",((int)PushAction.AddItem).ToString(CultureInfo.InvariantCulture)},
                    {"boxId", boxId.ToString(CultureInfo.InvariantCulture)}
                }, null);
            var applePushMessage = new ApplePushMessage();
            applePushMessage.Aps.AlertProperties.LocKey = "PUSH_NOTIFICATION_FILE";
            applePushMessage.Aps.AlertProperties["loc-args"] = new[]
                {
                    userNameOfAction, boxName
                
                };
            applePushMessage.Add("action", PushAction.AddItem);
            applePushMessage.Add("boxId", boxId);
            return SendNotificationAsync(googleMessage, applePushMessage, userIds);
        }

        public Task SendChatMessageNotificationAsync(string userNameOfAction,
            string text, Guid conversationId,
            IList<long> userIds)
        {
            var googleMessage = new GooglePushMessage(
             new Dictionary<string, string>
                {
                    {"text", text},
                    {"userName", userNameOfAction},
                    {"action",((int)PushAction.ChatMessage).ToString()},
                    {"conversationId", conversationId.ToString()}
                }, null);
            var applePushMessage = new ApplePushMessage();
            applePushMessage.Aps.AlertProperties.LocKey = "PUSH_NOTIFICATION_CHAT_MESSAGE";
            applePushMessage.Aps.AlertProperties["loc-args"] = new[]
                {
                    userNameOfAction, text

                };
            applePushMessage.Add("action", PushAction.ChatMessage);
            applePushMessage.Add("conversationId", conversationId);
            return SendNotificationAsync(googleMessage, applePushMessage, userIds);
        }

        public Task SendChatFileNotificationAsync(string userNameOfAction,Guid conversationId,
           IList<long> userIds)
        {
            var googleMessage = new GooglePushMessage(
             new Dictionary<string, string>
                {
                    {"userName", userNameOfAction},
                    {"action",((int)PushAction.ChatFile).ToString()},
                    {"conversationId", conversationId.ToString()}
                }, null);
            var applePushMessage = new ApplePushMessage();
            applePushMessage.Aps.AlertProperties.LocKey = "PUSH_NOTIFICATION_CHAT_FILE";
            applePushMessage.Aps.AlertProperties["loc-args"] = new[]
                {
                    userNameOfAction

                };
            applePushMessage.Add("action", PushAction.ChatFile);
            applePushMessage.Add("conversationId", conversationId);
            return SendNotificationAsync(googleMessage, applePushMessage, userIds);
        }



        public Task SendInviteNotificationAsync(string userNameOfAction,
            string boxName, long boxId,
            long userId)
        {
            var googleMessage = new GooglePushMessage(
             new Dictionary<string, string>
                {
                    {"boxName", boxName },
                    {"userName", userNameOfAction},
                    {"action",((int)PushAction.Invite).ToString(CultureInfo.InvariantCulture)},
                    {"boxId", boxId.ToString(CultureInfo.InvariantCulture)}
                }, null);
            var applePushMessage = new ApplePushMessage();
            applePushMessage.Aps.AlertProperties.LocKey = "PUSH_NOTIFICATION_INVITE";
            applePushMessage.Aps.AlertProperties["loc-args"] = new[]
                {
                    userNameOfAction, boxName
                
                };
            applePushMessage.Add("action", PushAction.Invite);
            applePushMessage.Add("boxId", boxId);
            return SendNotificationAsync(googleMessage, null, new[] { userId });
        }


        public async Task GetRegisteredUsersAsync()
        {
            var data = await m_Hub.GetAllRegistrationsAsync(50);
            foreach (var registrationDescription in data)
            {
                
            }
        }
    }
}
