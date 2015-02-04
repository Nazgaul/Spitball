using System;

using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Infrastructure.Azure.Queue
{
    public interface IQueueProviderExtract
    {
        Task<bool> RunQueue(QueueName queueName, Func<CloudQueueMessage, Task<bool>> func,
           TimeSpan invisibleTimeinQueue, int deQueueCount = 100);
    }
}
