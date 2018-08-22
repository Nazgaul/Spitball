﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Cloudents.Web.Hubs
{
    public class SbHub :Hub
    {

        public const string MethodName = "Message";
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync(MethodName, message);
        }
    }
}
