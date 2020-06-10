using Cloudents.Core.Message.Email;
using Cloudents.Core.Message.System;
using Cloudents.Core.Storage;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Cloudents.Core.Interfaces;

namespace Cloudents.Infrastructure.Storage
{
    public class QueueProvider : IQueueProvider
    {
        private readonly string _connectionString;
        //private readonly QueueClient _queueClient;
        private readonly IJsonSerializer _jsonSerializer;

        public QueueProvider(IConfigurationKeys connectionString, IJsonSerializer jsonSerializer)
        {
            _connectionString = connectionString.Storage;
            _jsonSerializer = jsonSerializer;
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

        public Task InsertBlobReprocessAsync(long id)
        {
            var queue = GetQueueReference("generate-blob-preview");
            return queue.SendMessageAsync(id.ToString());
        }

        public Task InsertMessageAsync(ISystemQueueMessage obj, TimeSpan delay, CancellationToken token)
        {
            var queue = GetQueueReference(QueueName.BackgroundQueue.Name);

            var json = _jsonSerializer.Serialize(obj);
            return queue.SendMessageAsync(json, delay, cancellationToken: token);
        }

      
    }
}