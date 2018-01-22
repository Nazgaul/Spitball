using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class IntercomApiManager : IIntercomApiManager
    {
        public async Task<IEnumerable<IntercomUsers>> GetUnsubscribersAsync(int page, CancellationToken token)
        {
            if (page < 1)
            {
                throw new ArgumentException("page need to be 1 or more");
            }
            using (var client = new HttpClient())
            {
                AddHeaders(client);
                using (
                    var response = await client.GetAsync($"https://api.intercom.io/users?segment_id=573177a948717f93ab00017a&page={page}", token))
                {
                    if (!response.IsSuccessStatusCode) return null;
                    using (var s = await response.Content.ReadAsStreamAsync())
                    {
                        using (var sr = new StreamReader(s))
                        {
                            using (var reader = new JsonTextReader(sr))
                            {
                                var obj = JObject.Load(reader);
                                var users = obj["users"];
                                return users.ToObject<IEnumerable<IntercomUsers>>();
                            }
                        }
                    }
                }
            }
        }

        //public async Task UpdateUserRefAsync(long userId, string email, string reference, CancellationToken token)
        //{
        //    using (var client = new HttpClient())
        //    {
        //        AddHeaders(client);
        //        var jsonString = JsonConvert.SerializeObject(new { user_id = userId, email, custom_attributes = new { reference } });
        //        using (var content = new StringContent(jsonString,Encoding.UTF8,"application/json"))
        //        {
        //            var response = await client.PostAsync("https://api.intercom.io/users", content, token);
        //            if (!response.IsSuccessStatusCode)
        //            {
        //                var text = await response.Content.ReadAsStringAsync();
        //                TraceLog.WriteError($"on update ref user intercom {text}");
        //            }
        //        }
        //    }
        //}

        private static void AddHeaders(HttpClient client)
        {
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
                    "bmptcGdheXY6ZWJmMWEyNWQyNjQ0YzUyZDM0NjA4OGJiODhhY2YzYjJjYTEzMjA0Mg==");
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }
    }


    public interface IIntercomApiManager
    {
        Task<IEnumerable<IntercomUsers>> GetUnsubscribersAsync(int page, CancellationToken token);
        //Task UpdateUserRefAsync(long userId, string email, string reference, CancellationToken token);
    }
}
