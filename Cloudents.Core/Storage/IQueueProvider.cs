using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Message;
using Cloudents.Core.Message.Email;
using Cloudents.Core.Message.System;

namespace Cloudents.Core.Storage
{
    public interface IQueueProvider
    {
        Task InsertMessageAsync(BaseEmail obj, CancellationToken token);
        Task InsertMessageAsync(BaseEmail obj,TimeSpan delay, CancellationToken token);
        //Task InsertMessageAsync(IEmailMessage obj, CancellationToken token);
        //Task InsertMessageAsync(SmsMessage2 obj, CancellationToken token);

        Task InsertMessageAsync(ISystemQueueMessage obj, CancellationToken token);

        Task InsertBlobReprocessAsync(long id);
    }

    public interface IServiceBusProvider
    {
        Task InsertMessageAsync(SmsMessage obj, CancellationToken token);
        //Task InsertMessageAsync(ISystemQueueMessage obj, CancellationToken token);
        //SignalRMessageTransport
        Task InsertMessageAsync(SignalRTransportType obj, long? userId, CancellationToken token);
        Task InsertMessageAsync(SignalRTransportType obj, string groupId, CancellationToken token);
        Task InsertMessageAsync(SignalRTransportType obj, CancellationToken token);

    }
}