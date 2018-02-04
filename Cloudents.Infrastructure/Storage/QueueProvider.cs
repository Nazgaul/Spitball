using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Cloudents.Infrastructure.Storage
{
    public class QueueProvider : IQueueProvider
    {
        private readonly CloudQueueClient _queueClient;
        private readonly IStreamSerializer _serializer;

        public QueueProvider(ICloudStorageProvider storageProvider, IStreamSerializer serializer)
        {
            _serializer = serializer;
            _queueClient = storageProvider.GetQueueClient();
        }

        public Task InsertMessageAsync<T>(QueueName name, T message, CancellationToken token)
        {
            var queue = _queueClient.GetQueueReference(name.Key.ToLower());
            var bytes = _serializer.Serialize(message);
            var cloudMessage = CloudQueueMessage.CreateCloudQueueMessageFromByteArray(bytes);
            return queue.AddMessageAsync(cloudMessage);
        }
    }
}
