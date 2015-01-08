﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.WorkerRole
{
    public class QueueProcess
    {
        private readonly IQueueProvider m_QueueProvider;
        public QueueProcess(IQueueProvider queueProvider, TimeSpan timeToThreadToSleepWhenQueueIsEmpty)
        {
            m_MinInterval = (int)timeToThreadToSleepWhenQueueIsEmpty.TotalSeconds;
            m_Interval = m_MinInterval;

            m_QueueProvider = queueProvider;
        }
        private const int Exponent = 2;
        private const int MaxInterval = 60;
        readonly int m_MinInterval;
        int m_Interval;

        public async Task RunQueue(QueueName queueName, Func<CloudQueueMessage, Task<bool>> func,
            TimeSpan invisibleTimeinQueue, int deQueueCount = 100)
        {
            var hasElementsToProcess = await m_QueueProvider.RunQueue(queueName, func, invisibleTimeinQueue, deQueueCount);
            if (hasElementsToProcess)
            {
                m_Interval = m_MinInterval;
            }
            else
            {
                await SleepAndIncreaseInterval();
            }
        }



        private async Task SleepAndIncreaseInterval()
        {
            //Thread.Sleep(TimeSpan.FromSeconds(m_Interval));
            await Task.Delay(TimeSpan.FromSeconds(m_Interval));
            m_Interval = Math.Min(MaxInterval, m_Interval * Exponent);
        }
    }
}
