﻿using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public static class SignalrClient
    {
        private static IHubProxy Proxy { get; set; }
        private static HubConnection _hub;

        public static async Task<IHubProxy> GetProxyAsync()
        {
            if (Proxy == null)
            {
                await InitConnectionAsync();
            }
            if (_hub.State == ConnectionState.Disconnected)
            {
                await InitConnectionAsync();
            }
            return Proxy;
        }

        private static async Task InitConnectionAsync()
        {
            _hub = new HubConnection(ConfigFetcher.Fetch("signalR") + "/s");
            Proxy = _hub.CreateHubProxy("SpitballHub");
            await _hub.Start();
            _hub.Closed += () =>
            {
                TraceLog.WriteInfo("need to reconnect");
                _hub.Start();
            };
        }
    }
}
