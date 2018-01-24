using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Zbang.Zbox.Infrastructure.Extensions;
using Cloudents.Core.Interfaces;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public static class SignalrClient
    {
        static IHubProxy Proxy { get; set; }
        private static HubConnection _hub;

        public static async Task<IHubProxy> GetProxyAsync()
        {
            if (Proxy == null)
            {
                await InitConnectionAsync().ConfigureAwait(false);
            }
            if (_hub.State == ConnectionState.Disconnected)
            {
                await InitConnectionAsync().ConfigureAwait(false);
            }
            if (_hub.State == ConnectionState.Connected)
            {
                return Proxy;
            }
            //TraceLog.WriteWarning($"SignalR is in state {_hub.State}");
            return null;
        }

        private static async Task InitConnectionAsync()
        {
            _hub = new HubConnection(ConfigFetcher.Fetch("signalR") + "/s");
            Proxy = _hub.CreateHubProxy("SpitballHub");
            await _hub.Start().ConfigureAwait(false);
            _hub.Closed += () =>
            {
                _hub.Start();
            };
        }
    }
}
