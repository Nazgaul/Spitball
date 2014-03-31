using System;
using Microsoft.WindowsAzure.Storage.Queue;
using Zbang.Zbox.Infrastructure.WebWorkerRoleJoinData.QueueDataTransfer;
using Zbang.Zbox.Infrastructure.Transport;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Storage
{
    public interface IQueueProvider
    {
        void InsertMessageToCache(FileProcessData message);
        void InsertMessageToMailNew(BaseMailData message);

       // void InsertMessageToThumbnail(GenerateThumbnail message);
        void InsertMessageToTranaction(DomainProcess message);
        Task InsertMessageToTranactionAsync(DomainProcess message);
        Task InsertMessageToDownloadAsync(UrlToDownloadData message);


        bool RunQueue(QueueName queueName, Func<CloudQueueMessage, bool> func,
           TimeSpan invisibleTimeinQueue, int dequeCount = 100);

       
    }

}
