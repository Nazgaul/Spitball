using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Request;

namespace Cloudents.Core.Storage
{
    public interface IServiceBusProvider
    {
        Task InsertMessageAsync(BaseEmail message, CancellationToken token);
        Task InsertMessageAsync(TalkJsUser message, CancellationToken token);
        Task InsertMessageAsync(BlockChainInitialBalance message, CancellationToken token);

        Task InsertMessageAsync(BlockChainQnaSubmit message, CancellationToken token);
    }
}