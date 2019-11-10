using Cloudents.Core.Message;
using Cloudents.Core.Message.Email;
using Cloudents.Core.Message.System;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Storage
{
    public interface IQueueProvider
    {
        Task InsertMessageAsync(BaseEmail obj, CancellationToken token);
        Task InsertMessageAsync(BaseEmail obj, TimeSpan delay, CancellationToken token);

        Task InsertMessageAsync(ISystemQueueMessage obj, CancellationToken token);

        Task InsertBlobReprocessAsync(long id);
        Task InsertMessageAsync(ISystemQueueMessage endTutoringSessionMessage, TimeSpan delay, CancellationToken token);
    }

    public interface IServiceBusProvider
    {
        Task InsertMessageAsync(SmsMessage obj, CancellationToken token);

        Task InsertMessageAsync(SignalRTransportType obj, long? userId, CancellationToken token);
        Task InsertMessageAsync(SignalRTransportType obj, string groupId, CancellationToken token);
        Task InsertMessageAsync(SignalRTransportType obj, CancellationToken token);

    }
}