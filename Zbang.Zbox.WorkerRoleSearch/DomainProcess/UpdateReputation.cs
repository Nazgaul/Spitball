using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRoleSearch.DomainProcess
{
    public class UpdateReputation : IDomainProcess
    {
        private readonly IZboxWorkerRoleService m_ZboxWriteService;
        private readonly ILogger _logger;

        public UpdateReputation(IZboxWorkerRoleService zboxWriteService, ILogger logger)
        {
            m_ZboxWriteService = zboxWriteService;
            _logger = logger;
        }

        public async Task<bool> ExecuteAsync(Infrastructure.Transport.DomainProcess data, CancellationToken token)
        {
            if (!(data is ReputationData parameters)) return true;
            try
            {
                //var userIds = string.Join(",", parameters.UserIds);
                //TraceLog.WriteInfo($"processing reputation for user {userIds}");
                var proxy = await SignalrClient.GetProxyAsync().ConfigureAwait(false);

                if (parameters.UserIds != null)
                    foreach (var userId in parameters.UserIds)
                    {
                        token.ThrowIfCancellationRequested();
                        var command = new UpdateReputationCommand(userId);
                        m_ZboxWriteService.UpdateReputation(command);
                        try
                        {
                            if (proxy != null) await proxy.Invoke("Score", command.Score, userId).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            _logger.Exception(ex, new Dictionary<string, string>
                            {
                                ["model"] = parameters.ToString(),
                                ["signalr"] = "signalr"
                            });
                        }
                    }
            }
            catch (Exception ex)
            {
                _logger.Exception(ex, new Dictionary<string, string>
                {
                    ["model"] = parameters.ToString()
                });
                return false;
            }
            return true;
        }
    }
}
