using Cloudents.Command;
using Cloudents.Core.Entities;
using Cloudents.Web.Extensions;
using Cloudents.Web.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command;

namespace Cloudents.Web.Hubs
{
    [Authorize]
    public class SbHub : Hub
    {
        private readonly Lazy<UserManager<RegularUser>> _userManager;

        public SbHub(Lazy<UserManager<RegularUser>> userManager)
        {
            _userManager = userManager;
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
            if (country != null)
            {

                await Groups.AddToGroupAsync(Context.ConnectionId, $"country_{country.ToLowerInvariant()}");
            }

            //await base.OnConnectedAsync();
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

            await base.OnDisconnectedAsync(exception);
        }


        public async Task ChatAsync(string message,
            long userId, [FromServices] ICommandBus bus, CancellationToken token)
        {
            var currentUserId = _userManager.Value.GetLongUserId(Context.User);
            if (userId == currentUserId)
            {
                return;
            }

            var command = new ChatAddMessageCommand(message, currentUserId, new[] { userId });
            var t1 =  bus.DispatchAsync(command, token);

            var t2 =  Clients.Users(new[] { currentUserId.ToString(), userId.ToString() }).SendAsync("Chat", new
            {
                message
            }, cancellationToken: token);
            await Task.WhenAll(t1, t2);
        }
    }
}
