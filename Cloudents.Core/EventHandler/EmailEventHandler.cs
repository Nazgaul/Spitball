using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.Message.Email;
using Cloudents.Application.Storage;
using Cloudents.Domain.Entities;

namespace Cloudents.Application.EventHandler
{
    public abstract class EmailEventHandler
    {
        private readonly IQueueProvider _serviceBusProvider;

        protected EmailEventHandler(IQueueProvider serviceBusProvider)
        {
            _serviceBusProvider = serviceBusProvider;
        }

        protected Task SendEmail(BaseEmail obj, RegularUser user, CancellationToken token)
        {
            if (!EmailValidate(user)) return Task.CompletedTask;
            return _serviceBusProvider.InsertMessageAsync(obj, token);

        }

        protected Task SendEmail(BaseEmail obj, TimeSpan delay, RegularUser user, CancellationToken token)
        {
            if (!EmailValidate(user)) return Task.CompletedTask;
            return _serviceBusProvider.InsertMessageAsync(obj, delay, token);

        }

        private static bool EmailValidate(RegularUser user)
        {
            if (!user.EmailConfirmed)
            {
                return false;
            }
            return true;
        }
    }
}
