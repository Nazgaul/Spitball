using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public static class SignalrClient
    {
        private static IHubProxy Proxy { get; set; }
        public static async Task<IHubProxy> GetProxyAsync()
        {
            if (Proxy == null)
            {
                var hubConnection = new HubConnection(ConfigFetcher.Fetch("signalR") + "/s");
                Proxy = hubConnection.CreateHubProxy("SpitballHub");
                await hubConnection.Start();
            }

            return Proxy;
        }


    }
}
