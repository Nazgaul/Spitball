using System;
using Microsoft.WindowsAzure.Storage.Queue;
using Zbang.Zbox.Infrastructure.Transport;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Storage
{
    public interface IQueueProvider
    {
        Task InsertMessageToStoreAsync(StoreData message);
        void InsertMessageToCache(FileProcessData message);

        Task InsertMessageToCacheAsync(FileProcessData message);

        void InsertMessageToMailNew(BaseMailData message);

        void InsertMessageToTranaction(DomainProcess message);
        Task InsertMessageToTranactionAsync(DomainProcess message);
        Task InsertMessageToDownloadAsync(UrlToDownloadData message);


        Task<bool> RunQueue(QueueName queueName, Func<CloudQueueMessage, Task<bool>> func,
            TimeSpan invisibleTimeinQueue, int deQueueCount = 100);


    }

}
