using System.Threading;
using Zbang.Zbox.Infrastructure.Transport;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Storage
{
    public interface IQueueProvider
    {
       // Task InsertMessageToCacheAsync(FileProcessData message);

        Task InsertMessageToMailNewAsync(BaseMailData message);

        Task InsertMessageToTransactionAsync(DomainProcess message);
        Task InsertMessageToTransactionAsync(DomainProcess message, CancellationToken token);

        Task InsertFileMessageAsync(FileProcess message);
    }
}
