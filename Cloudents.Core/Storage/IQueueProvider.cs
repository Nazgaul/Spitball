using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.Message;
using Cloudents.Application.Message.Email;
using Cloudents.Application.Message.System;

namespace Cloudents.Application.Storage
{
    public interface IQueueProvider
    {
        Task InsertMessageAsync(BaseEmail obj, CancellationToken token);
        Task InsertMessageAsync(BaseEmail obj,TimeSpan delay, CancellationToken token);
        //Task InsertMessageAsync(SmsMessage2 obj, CancellationToken token);

        Task InsertMessageAsync(ISystemQueueMessage obj, CancellationToken token);

        Task InsertBlobReprocessAsync(long id);
    }

    public interface IServiceBusProvider
    {
        Task InsertMessageAsync(SmsMessage2 obj, CancellationToken token);
        //Task InsertMessageAsync(ISystemQueueMessage obj, CancellationToken token);
        //SignalRMessageTransport
        Task InsertMessageAsync(SignalRTransportType obj, long? userId, CancellationToken token);
        Task InsertMessageAsync(SignalRTransportType obj, CancellationToken token);

    }
}