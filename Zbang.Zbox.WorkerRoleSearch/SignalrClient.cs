using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Trace;

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
                hubConnection.Closed += () =>
                {
                    TraceLog.WriteInfo("need to reconnect");
                    hubConnection.Start();
                };
            }
            return Proxy;
        }
    }
}
