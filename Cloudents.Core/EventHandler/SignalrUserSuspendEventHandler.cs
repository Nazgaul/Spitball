using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.EventHandler
{
    public class SignalrUserSuspendEventHandler : IEventHandler<UserSuspendEvent>, IEventHandler<DeleteUserEvent>
    {
        private readonly IServiceBusProvider _queueProvider;

        public SignalrUserSuspendEventHandler(IServiceBusProvider queueProvider)
        {
            _queueProvider = queueProvider;
        }

        public Task HandleAsync(UserSuspendEvent eventMessage, CancellationToken token)
        {
            return InsertMessageAsync(eventMessage.User.Id, token);
        }

        private Task InsertMessageAsync(long id, CancellationToken token)
        {
            var message = new SignalRTransportType(SignalRType.User,
                SignalRAction.Update, new {locakOutEnd = DateTime.MaxValue});

            return _queueProvider.InsertMessageAsync
                (message, id, token);
        }

        public  Task HandleAsync(DeleteUserEvent eventMessage, CancellationToken token)
        {
            return InsertMessageAsync(eventMessage.User.Id, token);
        }
    }
}
