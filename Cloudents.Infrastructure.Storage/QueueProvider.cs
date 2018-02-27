using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace Cloudents.Infrastructure.Storage
{
    public class QueueProvider : IQueueProvider
    {
        private readonly CloudQueueClient _queueClient;

        public QueueProvider(ICloudStorageProvider storageProvider)
        {
            _queueClient = storageProvider.GetQueueClient();
        }

        public Task InsertMessageAsync<T>(QueueName name, T message, CancellationToken token)
        {
            var queue = _queueClient.GetQueueReference(name.Key.ToLower());
            //TODO: is it right?
            var json = JsonConvert.SerializeObject(message);
            var cloudMessage = new CloudQueueMessage(json);
            return queue.AddMessageAsync(cloudMessage);
        }
    }
}
