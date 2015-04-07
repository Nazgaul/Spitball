using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Storage
{
    public interface ISendPush
    {
        Task SendAddItemNotification(string userNameOfAction,
            string boxName,
            IList<long> userIds);

        Task SendAddReplyNotification(string userNameOfAction,
            string text,
            string boxName,
            IList<long> userIds);

        Task SendAddPostNotification(string userNameOfAction,
            string text,
            string boxName,
            IList<long> userIds);

        Task SendInviteNotification(string userNameOfAction,
            string boxName,
            long userId);
    }
}