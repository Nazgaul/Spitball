﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Infrastructure.Azure.Queue
{
    public interface IQueueProviderExtract
    {
        Task<bool> RunQueueAsync(QueueName queueName, Func<CloudQueueMessage, Task<bool>> func,
           TimeSpan invisibleTimeinQueue, int deQueueCount = 100);


        Task<bool> RunQueueMultiple(QueueName queueName,
            Func<IEnumerable<CloudQueueMessage>, Task<IEnumerable<CloudQueueMessage>>> func,
            TimeSpan invisibleTimeinQueue, int deQueueCount = 100);
    }
}
