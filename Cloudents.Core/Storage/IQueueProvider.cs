using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Message;
using Cloudents.Core.Message.Email;
using Cloudents.Core.Message.System;
using Cloudents.Core.Storage.Dto;

namespace Cloudents.Core.Storage
{
    public interface IQueueProvider
    {
        Task InsertMessageAsync(NewQuestionMessage obj, CancellationToken token);
        Task InsertMessageAsync(BaseEmail obj, CancellationToken token);
        Task InsertMessageAsync(SmsMessage2 obj, CancellationToken token);

        Task InsertMessageAsync(ISystemQueueMessage obj, CancellationToken token);
    }
}