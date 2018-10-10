using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Storage;
using Cloudents.Core.Storage.Dto;
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

        public Task InsertQuestionMessageAsync(NewQuestionMessage obj, CancellationToken token)
        {
            return InsertMessageAsync(obj, QueueName.QuestionQueue, token);

        }

      

        private Task InsertMessageAsync(object obj, QueueName queueName, CancellationToken token)
        {
            var queue = _queueClient.GetQueueReference(queueName.Name);
            var json = JsonConvert.SerializeObject(obj);
            var cloudMessage = new CloudQueueMessage(json);
            token.ThrowIfCancellationRequested();
            return queue.AddMessageAsync(cloudMessage);
        }
    }
}