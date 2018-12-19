using JetBrains.Annotations;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.Message;
using Cloudents.Application.Message.Email;
using Cloudents.Application.Message.System;
using Cloudents.Application.Storage;

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



        public Task InsertMessageAsync(SmsMessage2 message, CancellationToken token)
        {

            return InsertMessageAsync(message, QueueName.SmsQueue, token);
        }

        public Task InsertMessageAsync(ISystemQueueMessage obj, CancellationToken token)
        {
            var queue = _queueClient.GetQueueReference(QueueName.BackgroundQueue.Name);
            var json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
            var cloudMessage = new CloudQueueMessage(json);
            token.ThrowIfCancellationRequested();
            return queue.AddMessageAsync(cloudMessage, null, null, new QueueRequestOptions(), new OperationContext(), token);
        }


        private Task InsertMessageAsync(object obj, QueueName queueName, CancellationToken token)
        {
            var queue = _queueClient.GetQueueReference(queueName.Name);
            var json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            var cloudMessage = new CloudQueueMessage(json);
            token.ThrowIfCancellationRequested();
            return queue.AddMessageAsync(cloudMessage);
        }


        public async Task InsertBlobReprocessAsync(long id)
        {
            var queue = _queueClient.GetQueueReference("generate-blob-preview");
            await queue.AddMessageAsync(new CloudQueueMessage(id.ToString()));
        }
    }
}