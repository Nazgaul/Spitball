using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

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
}