using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class DeleteOldConnections : IJob
    {
        private readonly IZboxWriteService m_ZboxWriteService;

        public DeleteOldConnections(IZboxWriteService zboxWriteService)
        {
            m_ZboxWriteService = zboxWriteService;
        }
        public string Name => nameof(DeleteOldConnections);

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                TraceLog.WriteInfo($"{Name} is doing process");
                var command = new RemoveOldConnectionCommand();
                try
                {

                    m_ZboxWriteService.RemoveOldConnections(command);
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError(Name, ex);
                }
                if (command.UserIds != null && command.UserIds.Any())
                {
                    try
                    {
                        var proxy = await SignalrClient.GetProxyAsync().ConfigureAwait(false);
                        foreach (var userId in command.UserIds)
                        {
                            if (proxy != null) await proxy.Invoke("Offline", userId).ConfigureAwait(false);
                        }
                    }
                    catch (Exception ex)
                    {
                        TraceLog.WriteError(Name, ex);
                    }
                }
                await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken).ConfigureAwait(false);
            }
        }

    }
}
