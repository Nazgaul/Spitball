﻿using Cloudents.Web.Identity;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Web.Hubs
{
    public class SbHub :Hub
    {
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
            if (Context.User.Identity.IsAuthenticated)
            {
                var country = Context.User.Claims.FirstOrDefault(f =>
                    string.Equals(f.Type, AppClaimsPrincipalFactory.Country.ToString(),
                        StringComparison.OrdinalIgnoreCase))?.Value;
                if (country != null)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"country-{country.ToLowerInvariant()}");
                }
            }

            //await base.OnConnectedAsync();
        }
    }
}
