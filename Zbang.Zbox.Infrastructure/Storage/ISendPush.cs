using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Storage
{
    public interface ISendPush
    {
        Task SendAddItemNotificationAsync(string userNameOfAction,
            string boxName, long boxId,
            IList<long> userIds);

        Task SendAddReplyNotificationAsync(string userNameOfAction,
            string text,
            string boxName, long boxId, Guid commentId,
            IList<long> userIds);

        Task SendAddPostNotificationAsync(string userNameOfAction,
            string text,
            string boxName, long boxId,
            IList<long> userIds);

        Task SendInviteNotificationAsync(string userNameOfAction,
            string boxName, long boxId,
            long userId);

        Task SendChatFileNotificationAsync(string userNameOfAction, Guid conversationId,
            IList<long> userIds);

        Task SendChatMessageNotificationAsync(string userNameOfAction,
            string text, Guid conversationId,
            IList<long> userIds);

        Task GetRegisteredUsersAsync();
    }

    public interface IJaredPushNotification
    {
        Task SendChatMessagePushAsync(string userName,
            string text, Guid conversationId,
            long userId);

        Task SendChatFilePushAsync(string userName, Guid conversationId,
            long userId);

        Task SendAddReplyPushAsync(string userName,
            string text,
            long boxId, Guid commentId,
            long userId);

        Task SendItemPushAsync(string userName,
            long boxId,long itemId,
            string tag);

        Task SendAddPostNotificationAsync(string userName,
            string text,
            long boxId, Guid feedId,
            string tag);
    }
}