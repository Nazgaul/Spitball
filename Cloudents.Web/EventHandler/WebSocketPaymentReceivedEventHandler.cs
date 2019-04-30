﻿using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Cloudents.Web.EventHandler
{
    public class WebSocketPaymentReceivedEventHandler : IEventHandler<StudentPaymentReceivedEvent>
    {
        private readonly IHubContext<SbHub> _hubContext;

        public WebSocketPaymentReceivedEventHandler(IHubContext<SbHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task HandleAsync(StudentPaymentReceivedEvent eventMessage, CancellationToken token)
        {
            var message = new SignalRTransportType(SignalRType.User,
                SignalREventAction.PaymentReceived,new object());
            await _hubContext.Clients.User(eventMessage.User.Id.ToString()).SendAsync(SbHub.MethodName, message, token);
        }
    }
}