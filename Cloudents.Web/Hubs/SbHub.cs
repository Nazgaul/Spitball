using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Web.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Interfaces;

namespace Cloudents.Web.Hubs
{
    [Authorize]
    public class SbHub : Hub
    {
        private readonly Lazy<ICommandBus> _commandBus;
        private readonly Lazy<ILogger> _logger;

        public SbHub(Lazy<ICommandBus> commandBus, Lazy<ILogger> logger)
        {
            _commandBus = commandBus;
            _logger = logger;
        }

        public const string MethodName = "Message";
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync(MethodName, message);
        }

        public async Task Message(object entity)
        {
            await Clients.All.SendAsync(MethodName, entity);
        }

        public override async Task OnConnectedAsync()
        {
            var country = Context.User.Claims.FirstOrDefault(f =>
                string.Equals(f.Type, AppClaimsPrincipalFactory.Country.ToString(),
                    StringComparison.OrdinalIgnoreCase))?.Value;

            var currentUserId = long.Parse(Context.UserIdentifier);

            try
            {
                var command = new ChangeOnlineStatusCommand(currentUserId, true);
                await _commandBus.Value.DispatchAsync(command, default);

            }
            catch (Exception e)
            {

                _logger.Value.Exception(e,new Dictionary<string, string>()
                {
                    ["SignalR"] = "Signalr"
                });
            }
            if (country != null)
            {

                await Groups.AddToGroupAsync(Context.ConnectionId, $"country_{country.ToLowerInvariant()}");
            }

            var message = new SignalRTransportType(SignalRType.User, SignalREventAction.OnlineStatus,
                new
                {
                    id = currentUserId,
                    online = true
                });



            var t2 = Clients.All.SendAsync(MethodName, message);


            await Task.WhenAll( t2);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var country = Context.User.Claims.FirstOrDefault(f =>
                string.Equals(f.Type, AppClaimsPrincipalFactory.Country.ToString(),
                    StringComparison.OrdinalIgnoreCase))?.Value;
            if (country != null)
            {

                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"country_{country.ToLowerInvariant()}");
            }

            var currentUserId = long.Parse(Context.UserIdentifier);
            try
            {
                var command = new ChangeOnlineStatusCommand(currentUserId, false);
                await _commandBus.Value.DispatchAsync(command, default);
            }
            catch (Exception e)
            {
                _logger.Value.Exception(e, new Dictionary<string, string>()
                {
                    ["SignalR"] = "Signalr"
                });
            }


            var message = new SignalRTransportType(SignalRType.User, SignalREventAction.OnlineStatus,
                new
                {
                    id = currentUserId,
                    online = false
                });



            var t2 = Clients.All.SendAsync(MethodName, message);
            await Task.WhenAll( t2);
            await base.OnDisconnectedAsync(exception);
        }


        //public async Task ChatAsync(string message, Guid? chatId,
        //    long userId,
        //    CancellationToken token)
        //{
        //    var currentUserId = _userManager.Value.GetLongUserId(Context.User);
        //    if (userId == currentUserId)
        //    {
        //        return;
        //    }

        //    var command = new SendMessageCommand(message, currentUserId, new[] { userId }, chatId);
        //    var t1 = _commandBus.Value.DispatchAsync(command, token);

        //    var t2 = Clients.Users(new[] { currentUserId.ToString(), userId.ToString() })
        //        .SendAsync("Chat", new
        //        {
        //            message
        //        }, cancellationToken: token);
        //    await Task.WhenAll(t1, t2);
        //}
    }
}
