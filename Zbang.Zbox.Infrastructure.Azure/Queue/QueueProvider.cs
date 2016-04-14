﻿using System;
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
    public class QueueProvider : IQueueProvider, IQueueProviderExtract
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

        public Task InsertMessageToCacheAsync(FileProcessData message)
        {
            var queue = GetCacheQueue();
            return queue.InsertToQueueProtoAsync(message);
        }

        public Task InsertMessageToMailNewAsync(BaseMailData message)
        {
            return GetMailQueueNew().InsertToQueueProtoAsync(message);
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

        public async Task<bool> RunQueueAsync(QueueName queueName, Func<CloudQueueMessage, Task<bool>> func,
           TimeSpan invisibleTimeinQueue, int deQueueCount = 100)
        {
            if (queueName == null) throw new ArgumentNullException(nameof(queueName));
            if (func == null) throw new ArgumentNullException(nameof(func));
            var queue = QueueClient.GetQueueReference(queueName.Name.ToLower());
            var messages = await queue.GetMessagesAsync(MaxQueuePopLimit, invisibleTimeinQueue, new QueueRequestOptions(), new OperationContext());
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
                        if (await func.Invoke(msg))
                        {
                            listToWait.Add(queue.DeleteMessageAsync(msg));
                        }
                    }
                    else
                    {
                        listToWait.Add(queue.DeleteMessageAsync(msg));
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
            await Task.WhenAll(listToWait);
            return cloudQueueMessages.Any();
        }

        //public async Task<bool> RunQueueMultiple(QueueName queueName,
        //    Func<IEnumerable<CloudQueueMessage>, Task<IEnumerable<CloudQueueMessage>>> func,
        //  TimeSpan invisibleTimeinQueue, int deQueueCount = 100)
        //{
        //    if (queueName == null) throw new ArgumentNullException(nameof(queueName));
        //    if (func == null) throw new ArgumentNullException(nameof(func));
        //    var queue = QueueClient.GetQueueReference(queueName.Name.ToLower());
        //    var messages = await queue.GetMessagesAsync(MaxQueuePopLimit, invisibleTimeinQueue, new QueueRequestOptions(), new OperationContext());
        //    if (messages == null)
        //    {
        //        return false;
        //    }
        //    var cloudQueueMessages = messages as IList<CloudQueueMessage> ?? messages.ToList();
        //    var listToWait = new List<Task>
        //    {
        //        queue.DeleteMessagesAsync(cloudQueueMessages.Where(w => w.DequeueCount < deQueueCount))
        //    };

        //    var messagesToDelete = await func.Invoke(cloudQueueMessages);
        //    listToWait.Add(queue.DeleteMessagesAsync(messagesToDelete));
        //    await Task.WhenAll(listToWait);
        //    return cloudQueueMessages.Any();
        //}



       
    }




}
