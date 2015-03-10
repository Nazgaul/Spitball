using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Notifications
{
    public interface ISendPush
    {
        Task SendAddItemNotification(string userNameOfAction,
            string itemName,
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
    }
}