using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class IntercomApiManager : IIntercomApiManager
    {
        public async Task<IEnumerable<IntercomUsers>> GetUnsubscribesAsync(int page, CancellationToken token)
        {
            if (page < 1)
            {
                throw new ArgumentException("page need to be 1 or more");
            }
            using (var client = new HttpClient())
            {
                AddHeaders(client);
                using (
                    var response = await client.GetAsync($"https://api.intercom.io/users?segment_id=573177a948717f93ab00017a&page={page}", token).ConfigureAwait(false))
                {
                    if (!response.IsSuccessStatusCode) return null;
                    using (var s = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    {
                        using (var sr = new StreamReader(s))
                        {
                            using (var reader = new JsonTextReader(sr))
                            {
                                var obj = await JObject.LoadAsync(reader, token).ConfigureAwait(false);
                                var users = obj["users"];
                                return users.ToObject<IEnumerable<IntercomUsers>>();
                            }
                        }
                    }
                }
            }
        }

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
        Task<IEnumerable<IntercomUsers>> GetUnsubscribesAsync(int page, CancellationToken token);
    }
}
