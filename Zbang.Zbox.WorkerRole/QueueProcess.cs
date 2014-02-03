using System;
using System.Linq;
using System.Threading;
//using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure.Storage.Queue;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Storage;
using System.Threading.Tasks;

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
        const int MaxQueuePopLimit = 32;
        private const int Exponent = 2;
        private const int MaxInterval = 60;
        readonly int m_MinInterval;
        int m_Interval;

        public void RunQueue(QueueName queueName, Func<CloudQueueMessage, bool> func,
            TimeSpan invisibleTimeinQueue, int dequeCount = 100)
        {
            var hasElementsToProcess = m_QueueProvider.RunQueue(queueName, func, invisibleTimeinQueue, dequeCount);
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
