using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Storage
{
    public interface ISendPush
    {
        Task SendAddItemNotification(string userNameOfAction,
            string boxName, long boxId,
            IList<long> userIds);

        Task SendAddReplyNotification(string userNameOfAction,
            string text,
            string boxName, long boxId, Guid commentId,
            IList<long> userIds);

        Task SendAddPostNotification(string userNameOfAction,
            string text,
            string boxName, long boxId,
            IList<long> userIds);

        Task SendInviteNotification(string userNameOfAction,
            string boxName, long boxId,
            long userId);
    }
}