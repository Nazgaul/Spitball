using Cloudents.Core.Message.Email;
using Cloudents.Core.Message.System;
using Cloudents.Core.Storage;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;

namespace Cloudents.Infrastructure.Storage
{
    public class QueueProvider : IQueueProvider
    {
        private readonly CloudQueueClient _queueClient;
        private readonly IJsonSerializer _jsonSerializer;

        public QueueProvider(ICloudStorageProvider storageProvider, IJsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer;
            _queueClient = storageProvider.GetQueueClient();
        }

        public Task InsertMessageAsync(BaseEmail obj, CancellationToken token)
        {
            return InsertMessageAsync(obj, TimeSpan.Zero, token);
        }

        public Task InsertMessageAsync(BaseEmail obj, TimeSpan delay, CancellationToken token)
        {
            var queue = _queueClient.GetQueueReference(QueueName.EmailQueue.Name);
            var json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
            var cloudMessage = new CloudQueueMessage(json);
            return queue.AddMessageAsync(cloudMessage, null, delay, new QueueRequestOptions(),
                new OperationContext(), token);
        }

        public Task InsertMessageAsync(ISystemQueueMessage obj, CancellationToken token)
        {
            return InsertMessageAsync(obj, TimeSpan.Zero, token);
        }

        public Task InsertBlobReprocessAsync(long id)
        {
            var queue = _queueClient.GetQueueReference("generate-blob-preview");
            return queue.AddMessageAsync(new CloudQueueMessage(id.ToString()));
        }

        public Task InsertMessageAsync(ISystemQueueMessage obj, TimeSpan delay, CancellationToken token)
        {
            var queue = _queueClient.GetQueueReference(QueueName.BackgroundQueue.Name);

            var json = _jsonSerializer.Serialize(obj);
            var cloudMessage = new CloudQueueMessage(json);
            token.ThrowIfCancellationRequested();
            return queue.AddMessageAsync(cloudMessage, null, delay, new QueueRequestOptions(), new OperationContext(), token);
        }

      
    }
}