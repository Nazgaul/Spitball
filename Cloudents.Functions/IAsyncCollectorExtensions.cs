using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;

namespace Cloudents.Functions
{
    public static class AsyncCollectorExtensions
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

        public static Task AddAsStringAsync<T>(this IAsyncCollector<string> collector, T item, CancellationToken token)
        {
            var serialized = JsonConvert.SerializeObject(item, Settings);
            return collector.AddAsync(serialized, token);
        }
    }
}