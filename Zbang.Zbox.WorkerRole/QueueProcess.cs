using System;
using System.Threading;
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

        public void RunQueue(QueueName queueName, Func<CloudQueueMessage, bool> func,
            TimeSpan invisibleTimeinQueue, int deQueueCount = 100)
        {
            var hasElementsToProcess = m_QueueProvider.RunQueue(queueName, func, invisibleTimeinQueue, deQueueCount);
            if (hasElementsToProcess)
            {
                m_Interval = m_MinInterval;
            }
            else
            {
                SleepAndIncreaseInterval();
            }
        }
        


        private void SleepAndIncreaseInterval()
        {
            Thread.Sleep(TimeSpan.FromSeconds(m_Interval));
            m_Interval = Math.Min(MaxInterval, m_Interval * Exponent);
        }
    }
}
