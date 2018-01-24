using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Zbang.Zbox.Infrastructure.Azure.Storage;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Infrastructure.Azure.Queue
{
    public class QueueProvider : IQueueProvider, IQueueProviderExtract
    {
        private readonly ILogger _logger;
        private CloudQueueClient _queueClient;

        public QueueProvider(ILogger logger)
        {
            _logger = logger;
        }

        private const int MaxQueuePopLimit = 32;

        private CloudQueueClient QueueClient => _queueClient ?? (_queueClient = StorageProvider.ZboxCloudStorage.CreateCloudQueueClient());

        private CloudQueue GetTransactionQueue()
        {
            return GetQueue(QueueName.UpdateDomainQueueName.ToLower());
        }

        public Task InsertMessageToMailNewAsync(BaseMailData message)
        {
            var queue = GetQueue(QueueName.MailQueueName.ToLower());
            return queue.InsertToQueueProtoAsync(message);
        }

        public Task InsertMessageToTransactionAsync(DomainProcess message)
        {
            return GetTransactionQueue().InsertToQueueProtoAsync(message);
        }

        public Task InsertMessageToTransactionAsync(DomainProcess message, CancellationToken token)
        {
            return GetTransactionQueue().InsertToQueueProtoAsync(message, token);
        }

        public Task InsertFileMessageAsync(FileProcess message)
        {
            var queue = GetQueue(QueueName.ThumbnailQueueName.ToLower());
            return queue.InsertToQueueProtoAsync(message);
        }

        public CloudQueue GetQueue(string queueName)
        {
            return QueueClient.GetQueueReference(queueName.ToLower());
        }

        public async Task<bool> RunQueueAsync(QueueName queueName, Func<CloudQueueMessage, Task<bool>> func,
           TimeSpan invisible, int deQueueCount, CancellationToken token)
        {
            if (queueName == null) throw new ArgumentNullException(nameof(queueName));
            if (func == null) throw new ArgumentNullException(nameof(func));
            var queue = QueueClient.GetQueueReference(queueName.Name.ToLowerInvariant());
            var messages = await queue.GetMessagesAsync(MaxQueuePopLimit, invisible, new QueueRequestOptions(), new OperationContext(), token).ConfigureAwait(false);
            if (messages == null)
            {
                return false;
            }
            var cloudQueueMessages = messages as IList<CloudQueueMessage> ?? messages.ToList();
            var listToWait = new List<Task>();
            foreach (var msg in cloudQueueMessages)
            {
                try
                {
                    if (msg.DequeueCount < deQueueCount)
                    {
                        if (await func.Invoke(msg).ConfigureAwait(false))
                        {
                            listToWait.Add(queue.DeleteMessageAsync(msg, token));
                        }
                    }
                    else
                    {
                        listToWait.Add(queue.DeleteMessageAsync(msg, token));
                    }
                }
                catch (StorageException ex)
                {
                    _logger.Exception(ex, new Dictionary<string, string>
                    {
                        ["Queue"] = queue.Name,
                        ["MessageId"] = msg.Id,
                        ["DeQueue"] = msg.DequeueCount.ToString()
                    });
                }
                catch (Exception ex)
                {
                    _logger.Exception(ex, new Dictionary<string, string>
                    {
                        ["Queue"] = queue.Name,
                        ["MessageId"] = msg.Id,
                        ["DeQueue"] = msg.DequeueCount.ToString()
                    });
                }
            }
            await Task.WhenAll(listToWait).ConfigureAwait(false);
            return cloudQueueMessages.Count > 0;
        }

        public Task UpdateMessageAsync(QueueName queueName, CloudQueueMessage msg, TimeSpan visibilityTimeout, CancellationToken token)
        {
            var queue = QueueClient.GetQueueReference(queueName.Name.ToLower());
            return queue.UpdateMessageAsync(msg, visibilityTimeout,
                MessageUpdateFields.Content | MessageUpdateFields.Visibility, token);
        }
    }
}
