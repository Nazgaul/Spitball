using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Message;

namespace Cloudents.Core.Storage
{
    public interface IServiceBusProvider
    {
        //Task InsertMessageAsync(BaseEmail message, CancellationToken token);
        Task InsertMessageAsync(UrlRedirectQueueMessage message, CancellationToken token);
        //Task InsertMessageAsync(SmsMessage2 message, CancellationToken token);

    }
}