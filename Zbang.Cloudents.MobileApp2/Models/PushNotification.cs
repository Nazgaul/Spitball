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
        public ApiServices Services { get; set; }

        public IZboxCacheReadService ZboxReadService { get; set; }

        public async Task SendPush(long boxId)
        {
            var page = 0;
            var data = new Dictionary<string, string>()
            {
                {"message", "some text"}
            };
            var message = new GooglePushMessage(data, null);
            var ids = await GetUserIds(boxId, page);
            while (ids.Any())
            {
                try
                {
                    var result =
                        await
                            Services.Push.SendAsync(message,
                                18372.ToString(CultureInfo.InvariantCulture));
                    Services.Log.Info(result.State.ToString());
                    page++;
                    ids = await GetUserIds(boxId, page);
                }
                catch (Exception ex)
                {
                    Services.Log.Error(ex.Message, null, "Push.SendAsync Error");
                }
            }

        }

        private async Task<IList<long>> GetUserIds(long boxId, int page)
        {
            var userIds = await ZboxReadService.GetBoxUsersId(new Zbox.ViewModel.Queries.GetBoxQuery(boxId, page, 20));
            return userIds as IList<long> ?? userIds.ToList();
        }
    }

    public interface IPushNotification
    {
        Task SendPush(long boxId);
    }
}