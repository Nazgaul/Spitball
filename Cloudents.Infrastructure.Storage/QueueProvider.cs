using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Storage;
using JetBrains.Annotations;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace Cloudents.Infrastructure.Storage
{
    [UsedImplicitly]
    public class QueueProvider : IQueueProvider
    {
        private readonly CloudQueueClient _queueClient;

        public QueueProvider(ICloudStorageProvider storageProvider)
        {
            _queueClient = storageProvider.GetQueueClient();
        }

        public Task InsertMessageAsync<T>(T message, CancellationToken token) where T : IQueueName
        {
            var queue = _queueClient.GetQueueReference(message.QueueName.Key.ToLower());
            var json = JsonConvert.SerializeObject(message);
            var cloudMessage = new CloudQueueMessage(json);
            return queue.AddMessageAsync(cloudMessage);
        }
    }
}
