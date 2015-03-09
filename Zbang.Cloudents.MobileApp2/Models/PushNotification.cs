using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using Zbang.Zbox.ReadServices;

namespace Zbang.Cloudents.MobileApp2.Models
{
    public class PushNotification : IPushNotification
    {
        private readonly ApiServices m_Services;
        private readonly IZboxCacheReadService m_ZboxReadService;


        public PushNotification(ApiServices services, IZboxCacheReadService zboxReadService)
        {
            m_Services = services;
            m_ZboxReadService = zboxReadService;
        }
        public async Task SendPush(long boxId)
        {
            var page = 0;
            var data = new Dictionary<string, string>
            {
                {"message", "boxid: " + boxId + " was updated" }
            };
            var message = new GooglePushMessage(data, null);
            var ids = await GetUserIds(boxId, page);

            while (ids.Any())
            {
                try
                {
                    var userid = page % 2 == 0 ? 1 : 18372;
                    await
                        m_Services.Push.SendAsync(message,
                            userid.ToString(CultureInfo.InvariantCulture));
                    m_Services.Log.Info("sending message to: " + userid);
                    page++;
                    ids = await GetUserIds(boxId, page);
                }
                catch (Exception ex)
                {
                    m_Services.Log.Error(ex.Message, null, "Push.SendAsync Error");
                }

            }

        }

        private async Task<IList<long>> GetUserIds(long boxId, int page)
        {
            var userIds = await m_ZboxReadService.GetBoxUsersId(new Zbox.ViewModel.Queries.GetBoxQuery(boxId, page, 20));
            return userIds as IList<long> ?? userIds.ToList();
        }
    }

    public interface IPushNotification
    {
        Task SendPush(long boxId);
    }
}