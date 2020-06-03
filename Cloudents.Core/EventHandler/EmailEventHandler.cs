using Cloudents.Core.Message.Email;
using Cloudents.Core.Storage;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.EventHandler
{
    public abstract class EmailEventHandler
    {
        private readonly IQueueProvider _serviceBusProvider;

        protected EmailEventHandler(IQueueProvider serviceBusProvider)
        {
            _serviceBusProvider = serviceBusProvider;
        }

        protected Task SendEmailAsync(BaseEmail obj, CancellationToken token)
        {
            return _serviceBusProvider.InsertMessageAsync(obj, token);

        }

        //protected Task SendEmail(BaseEmail obj, TimeSpan delay, CancellationToken token)
        //{
        //    return _serviceBusProvider.InsertMessageAsync(obj, delay, token);

        //}


    }
}
