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
        private const int UsersPerPage = 20;

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
                {"message", "boxid: " + boxId + " was updated" },
                {"title", "some title"}
            };
            var message = new GooglePushMessage(data, null);
            var ids = await GetUserIds(boxId, page);
            var list = new List<Task>();

            //while (ids.Any())
            //{
                try
                {
                    list.Add(
                        m_Services.Push.SendAsync(message,
                            1.ToString(CultureInfo.InvariantCulture)));
                    list.Add(
                        m_Services.Push.SendAsync(message,
                            18372.ToString(CultureInfo.InvariantCulture)));
                    page++;
                    if (ids.Count < UsersPerPage)
                    {
                       // break;
                    }
                    //ids = await GetUserIds(boxId, page);
                }
                catch (Exception ex)
                {
                    m_Services.Log.Error(ex.Message, null, "Push.SendAsync Error");
                }

            //}
            await Task.WhenAll(list);

        }

        private async Task<IList<long>> GetUserIds(long boxId, int page)
        {
            var userIds = await m_ZboxReadService.GetBoxUsersId(new Zbox.ViewModel.Queries.GetBoxQuery(boxId, page, UsersPerPage));
            return userIds as IList<long> ?? userIds.ToList();
        }
    }

    public interface IPushNotification
    {
        Task SendPush(long boxId);
    }
}