using Zbang.Zbox.Infrastructure.Transport;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Storage
{
    public interface IQueueProvider
    {
        Task InsertMessageToStoreAsync(StoreData message);

        Task InsertMessageToCacheAsync(FileProcessData message);

        Task InsertMessageToMailNewAsync(BaseMailData message);

        void InsertMessageToTranaction(DomainProcess message);
        Task InsertMessageToTranactionAsync(DomainProcess message);
        Task InsertMessageToDownloadAsync(UrlToDownloadData message);
    }

}
