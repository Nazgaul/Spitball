using Cloudents.Core.Message.Email;
using Cloudents.Core.Message.System;
using Cloudents.Core.Storage;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Queues;

namespace Cloudents.Infrastructure.Storage
{
    public class QueueProvider : IQueueProvider
    {
        private readonly string _connectionString;
        //private readonly QueueClient _queueClient;

        public QueueProvider(string connectionString)
        {
            _connectionString = connectionString;

            // _queueClient = storageProvider.GetQueueClient();
        }

        public Task InsertMessageAsync(BaseEmail obj, CancellationToken token)
        {
            return InsertMessageAsync(obj, TimeSpan.Zero, token);
        }

        private QueueClient GetQueueReference(string queueName)
        {
            return new QueueClient(_connectionString, queueName);

        }

        public Task InsertMessageAsync(BaseEmail obj, TimeSpan delay, CancellationToken token)
        {
            var queue = new QueueClient(_connectionString, QueueName.EmailQueue.Name);
            //var queue = _queueClient.GetQueueReference(QueueName.EmailQueue.Name);
            var json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
            //var cloudMessage = new CloudQueueMessage(json);
            return queue.SendMessageAsync(json,delay, cancellationToken: token);
            //return queue.AddMessageAsync(cloudMessage, null, delay, new QueueRequestOptions(),
            //    new OperationContext(), token);
        }

        public Task InsertMessageAsync(ISystemQueueMessage obj, CancellationToken token)
        {
            return InsertMessageAsync(obj, TimeSpan.Zero, token);
        }

        public async Task InsertBlobReprocessAsync(long id)
        {
            var queue = GetQueueReference("generate-blob-preview");
            await queue.SendMessageAsync(id.ToString());
        }

        public Task InsertMessageAsync(ISystemQueueMessage obj, TimeSpan delay, CancellationToken token)
        {
            var queue = GetQueueReference(QueueName.BackgroundQueue.Name);
            var json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
            return queue.SendMessageAsync(json, delay, cancellationToken: token);
        }
    }
}