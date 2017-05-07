using System;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Notifications;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Infrastructure.Notifications
{
    public class JaredSendPush : IJaredPushNotification
    {
        private readonly NotificationHubClient m_Hub;
        public JaredSendPush(string connectionString, string hubName)
        {
            if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(hubName))
            {
                return;
            }
            m_Hub = NotificationHubClient.CreateClientFromConnectionString(connectionString, hubName);
        }

        public Task SendChatMessagePushAsync(string userName, string text, Guid conversationId, long userId)
        {
            var applePushMessage = new ApplePushMessage();
            applePushMessage.Aps.AlertProperties.Title = "Chat message";
            applePushMessage.Aps.AlertProperties.Body = $"{userName} send  you a {text ?? "file"} ";
            applePushMessage.Add("action", PushAction.ChatMessage);
            applePushMessage.Add("conversationId", conversationId);
            return SendNotificationAsync(new AppleNotification(applePushMessage.ToString()), $"_UserId:{userId}");
        }

        public Task SendChatFilePushAsync(string userName, Guid conversationId, long userId)
        {
            var applePushMessage = new ApplePushMessage();
            applePushMessage.Aps.AlertProperties.Title = "Chat message";
            applePushMessage.Aps.AlertProperties.Body = $"{userName} send  you a file";
            applePushMessage.Add("action", PushAction.ChatFile);
            applePushMessage.Add("conversationId", conversationId);
            return SendNotificationAsync(new AppleNotification(applePushMessage.ToString()), $"_UserId:{userId}");
        }

        public Task SendAddReplyPushAsync(string userName, string text, long boxId, Guid commentId, long userId)
        {
            var applePushMessage = new ApplePushMessage();
            applePushMessage.Aps.AlertProperties.Title = "Reply";
            applePushMessage.Aps.AlertProperties.Body = $"{userName} send replied to your question";
            applePushMessage.Add("action", PushAction.PostReply);
            applePushMessage.Add("boxId", boxId);
            applePushMessage.Add("commentId", commentId);
            return SendNotificationAsync(new AppleNotification(applePushMessage.ToString()), $"_UserId:{userId}");
        }

        public Task SendItemPushAsync(string userName, long boxId, long itemId, string tag)
        {
            var applePushMessage = new ApplePushMessage();
            applePushMessage.Aps.AlertProperties.Title = "File";
            applePushMessage.Aps.AlertProperties.Body = $"{userName} uploaded something you might be interested";
            applePushMessage.Add("action", PushAction.AddItem);
            applePushMessage.Add("boxId", boxId);
            applePushMessage.Add("itemId", itemId);

            return SendNotificationAsync(new AppleNotification(applePushMessage.ToString()), tag);
        }

        public Task SendAddPostNotificationAsync(string userName, string text, long boxId, Guid feedId, string tag)
        {
            var applePushMessage = new ApplePushMessage();
            applePushMessage.Aps.AlertProperties.Title = "Ask question";
            applePushMessage.Aps.AlertProperties.Body = $"{userName} ask you care to answer?";

            applePushMessage.Add("action", PushAction.PostComment);
            applePushMessage.Add("boxId", boxId);
            applePushMessage.Add("feedId", feedId);
            return SendNotificationAsync(new AppleNotification(applePushMessage.ToString()), tag);
        }

        private Task SendNotificationAsync(Notification message, string tag)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (tag == null) throw new ArgumentNullException(nameof(tag));

            if (m_Hub == null)
            {
                return Task.CompletedTask;
            }

            if (ConfigFetcher.IsEmulated)
            {
                return Task.FromResult(false);
            }
            var hub = NotificationHubClient.CreateClientFromConnectionString("Endpoint=sb://spitball.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=1+AAf2FSzauWHpYhHaoweYT9576paNgmicNSv6jAvKk=", "spitball");
            return hub.SendNotificationAsync(message, tag);
        }
       
    }
}
