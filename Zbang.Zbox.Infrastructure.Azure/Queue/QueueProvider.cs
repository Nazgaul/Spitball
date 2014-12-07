using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Zbang.Zbox.Infrastructure.Azure.Storage;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Infrastructure.Azure.Queue
{
    public class QueueProvider : IQueueProvider
    {

        private CloudQueueClient m_QueueClient;

        const int MaxQueuePopLimit = 32;

        private CloudQueueClient QueueClient
        {
            get { return m_QueueClient ?? (m_QueueClient = StorageProvider.ZboxCloudStorage.CreateCloudQueueClient()); }
        }


        private CloudQueue GetCacheQueue()
        {
            return GetQueue(QueueName.QueueName2.ToLower());
        }

        private CloudQueue GetMailQueueNew()
        {
            return GetQueue(QueueName.NewMailQueueName.ToLower());
        }
        private CloudQueue GetDownloadContentFromUrlQueue()
        {
            return GetQueue(QueueName.DownloadContentFromUrl.ToLower());
        }



        private CloudQueue GetTransactionQueue()
        {
            return GetQueue(QueueName.UpdateDomainQueueName.ToLower());
        }

        public void InsertMessageToCache(FileProcessData message)
        {
            var queue = GetCacheQueue();
            queue.InsertToQueueProto(message);
        }

        public Task InsertMessageToCacheAsync(FileProcessData message)
        {
            var queue = GetCacheQueue();
            return queue.InsertToQueueProtoAsync(message);
        }

        public void InsertMessageToMailNew(BaseMailData message)
        {
            GetMailQueueNew().InsertToQueueProto(message);
        }


        public void InsertMessageToTranaction(DomainProcess message)
        {
            GetTransactionQueue().InsertToQueueProto(message);
        }

        public Task InsertMessageToTranactionAsync(DomainProcess message)
        {
            return GetTransactionQueue().InsertToQueueProtoAsync(message);
        }

        public async Task InsertMessageToDownloadAsync(UrlToDownloadData message)
        {
            await GetDownloadContentFromUrlQueue().InsertToQueueProtoAsync(message);
        }



        private CloudQueue GetQueue(string queueName)
        {
            var queue = QueueClient.GetQueueReference(queueName.ToLower());
            return queue;
        }

        public bool RunQueue(QueueName queueName, Func<CloudQueueMessage, bool> func,
           TimeSpan invisibleTimeinQueue, int deQueueCount = 100)
        {
            if (queueName == null) throw new ArgumentNullException("queueName");
            if (func == null) throw new ArgumentNullException("func");
            var queue = QueueClient.GetQueueReference(queueName.Name.ToLower());
            var messages = queue.GetMessages(MaxQueuePopLimit, invisibleTimeinQueue);
            if (messages == null)
            {
                //SleepAndIncreaseInterval();
                return false;
            }
            var cloudQueueMessages = messages as IList<CloudQueueMessage> ?? messages.ToList();
            foreach (var msg in cloudQueueMessages)
            {
                try
                {
                    //m_Interval = m_MinInterval;

                    if (msg.DequeueCount < deQueueCount)
                    {
                        if (func.Invoke(msg))
                        {
                            queue.DeleteMessage(msg);
                        }
                    }
                    else
                    {
                        queue.DeleteMessage(msg);
                    }
                }
                catch (StorageException ex)
                {
                    TraceLog.WriteError("Queue: " + queue.Name + " run " + msg.Id + " DeQueue count: " + msg.DequeueCount, ex);
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError("Queue: " + queue.Name + " run " + msg.Id + " DeQueue count: " + msg.DequeueCount, ex);
                }
            }
            if (!cloudQueueMessages.Any())
            {
                return false;
            }
            return true;
        }



        public async Task InsertMessageToStoreAsync(StoreData message)
        {
            await QueueClient.GetQueueReference(QueueName.OrderQueueName.ToLower()).InsertToQueueProtoAsync(message);
        }
    }




}
