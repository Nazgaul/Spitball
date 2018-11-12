using Cloudents.Core.Entities.Db;
using Cloudents.Core.Message.Email;
using Cloudents.Core.Storage;
using System;
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

        protected Task SendEmail(BaseEmail obj, User user, CancellationToken token)
        {
            if (!EmailValidate(user)) return Task.CompletedTask;
            return _serviceBusProvider.InsertMessageAsync(obj, token);

        }

        protected Task SendEmail(BaseEmail obj, TimeSpan delay, User user, CancellationToken token)
        {
            if (!EmailValidate(user)) return Task.CompletedTask;
            return _serviceBusProvider.InsertMessageAsync(obj, delay, token);

        }

        private bool EmailValidate(User user)
        {
            if (!user.EmailConfirmed || user.Fictive)
            {
                return false;
            }
            return true;
        }
    }
}
