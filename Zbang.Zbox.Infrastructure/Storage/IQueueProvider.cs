using System.Threading;
using Zbang.Zbox.Infrastructure.Transport;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Storage
{
    public interface IQueueProvider
    {

       // Task InsertMessageToCacheAsync(FileProcessData message);

        Task InsertMessageToMailNewAsync(BaseMailData message);

        Task InsertMessageToTranactionAsync(DomainProcess message);
        Task InsertMessageToTranactionAsync(DomainProcess message, CancellationToken token);
        //Task InsertMessageToDownloadAsync(UrlToDownloadData message);

        Task InsertMessageToThumbnailAsync(FileProcess message);
    }

}
