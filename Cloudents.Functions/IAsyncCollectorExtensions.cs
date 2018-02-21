using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;

namespace Cloudents.Functions
{
    public static class AsyncCollectorExtensions
    {

        public static Task AddAsStringAsync<T>(this IAsyncCollector<string> collector, T item, CancellationToken token)
        {
            var serialized = JsonConvertInheritance.SerializeObject(item);
            return collector.AddAsync(serialized, token);
        }
    }

    public static class JsonConvertInheritance
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

        public static string SerializeObject(object value)
        {
            return JsonConvert.SerializeObject(value, Settings);
        }
        public static T DeserializeObject<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value, Settings);
        }
    }
}