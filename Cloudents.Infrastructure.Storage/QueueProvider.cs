using Cloudents.Core.Message;
using Cloudents.Core.Storage;
using Cloudents.Core.Storage.Dto;
using JetBrains.Annotations;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;

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

        /// <summary>
        /// Insert new question from admin to queue-  this is different because of the nature of the pool
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task InsertMessageAsync(NewQuestionMessage obj, CancellationToken token)
        {
            return InsertMessageAsync(obj, QueueName.QuestionQueue, token);

        }

        public Task InsertMessageAsync(BaseEmail message, CancellationToken token)
        {
            var queue = _queueClient.GetQueueReference(QueueName.EmailQueue.Name);
            var json = JsonConvert.SerializeObject(message, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
            var cloudMessage = new CloudQueueMessage(json);
            token.ThrowIfCancellationRequested();
            return queue.AddMessageAsync(cloudMessage);
        }

        public Task InsertMessageAsync(SmsMessage2 message, CancellationToken token)
        {

            return InsertMessageAsync(message, QueueName.SmsQueue, token);
        }






        private Task InsertMessageAsync(object obj, QueueName queueName, CancellationToken token)
        {
            var queue = _queueClient.GetQueueReference(queueName.Name);
            var json = JsonConvert.SerializeObject(obj,new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            var cloudMessage = new CloudQueueMessage(json);
            token.ThrowIfCancellationRequested();
            return queue.AddMessageAsync(cloudMessage);
        }
    }
}