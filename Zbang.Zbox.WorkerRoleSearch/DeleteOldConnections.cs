using System;
using System.Collections.Generic;
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
        private readonly ILogger m_Logger;

        public DeleteOldConnections(IZboxWriteService zboxWriteService, ILogger logger)
        {
            m_ZboxWriteService = zboxWriteService;
            m_Logger = logger;
        }
        public string Name => nameof(DeleteOldConnections);

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var index = RoleIndexProcessor.GetIndex();
            if (index != 0)
            {
                return;
            }
            while (!cancellationToken.IsCancellationRequested)
            {
                var command = new RemoveOldConnectionCommand();
                try
                {

                    m_ZboxWriteService.RemoveOldConnections(command);
                }
                catch (Exception ex)
                {
                    m_Logger.Exception(ex, new Dictionary<string,string> {{"process", Name}});
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
                        m_Logger.Exception(ex, new Dictionary<string, string> { { "process", Name } });
                    }
                }
                await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken).ConfigureAwait(false);
            }
        }

    }
}
