﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Infrastructure.Azure.Queue
{
    public interface IQueueProviderExtract
    {
        Task<bool> RunQueueAsync(QueueName queueName, Func<CloudQueueMessage, Task<bool>> func,
           TimeSpan invisibleTimeinQueue, int deQueueCount = 100);

        Task<bool> RunQueueAsync(QueueName queueName, Func<CloudQueueMessage, Task<bool>> func,
            TimeSpan invisibleTimeinQueue, int deQueueCount, CancellationToken token);
        Task UpdateMessageAsync(QueueName queueName, CloudQueueMessage msg, CancellationToken token);
        
    }
}
