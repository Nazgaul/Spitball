using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class DeleteOldConnections : IJob
    {
        private readonly IZboxWriteService _zboxWriteService;
        private readonly ILogger _logger;

        public DeleteOldConnections(IZboxWriteService zboxWriteService, ILogger logger)
        {
            _zboxWriteService = zboxWriteService;
            _logger = logger;
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
                    _zboxWriteService.RemoveOldConnections(command);
                }
                catch (Exception ex)
                {
                    _logger.Exception(ex, new Dictionary<string,string> {["process"] = Name });
                }
                if (command.UserIds?.Any() == true)
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
                        _logger.Exception(ex, new Dictionary<string, string> {["process"] = Name });
                    }
                }
                await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
