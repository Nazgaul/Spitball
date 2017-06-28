using System;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Infrastructure.Storage
{
    public interface IJaredPushNotification
    {
        Task SendChatMessagePushAsync(string userName,
            string text, Guid conversationId,
            long conversationUser, long userToSendId);

        //Task SendChatFilePushAsync(string userName, Guid conversationId,
        //    long conversationUser, long userToSendId);

        Task SendAddReplyPushAsync(string userName,
            string text,
            long boxId, Guid commentId,
            long userId);

        Task SendItemPushAsync(string userName, long boxId, long itemId, string tag, ItemType type);

        Task SendAddPostNotificationAsync(string userName,
            string text,
            long boxId, Guid feedId,
            string tag);
    }
}