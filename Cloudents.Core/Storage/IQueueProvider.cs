using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Message;
using Cloudents.Core.Storage.Dto;

namespace Cloudents.Core.Storage
{
    public interface IQueueProvider
    {
        Task InsertMessageAsync(NewQuestionMessage obj, CancellationToken token);
        Task InsertMessageAsync(BaseEmail obj, CancellationToken token);
        Task InsertMessageAsync(SmsMessage2 obj, CancellationToken token);
    }
}