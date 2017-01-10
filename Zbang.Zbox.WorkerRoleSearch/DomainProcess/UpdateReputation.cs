using System;
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

        public UpdateReputation(IZboxWorkerRoleService zboxWriteService)
        {
            m_ZboxWriteService = zboxWriteService;
        }

        public async Task<bool> ExecuteAsync(Infrastructure.Transport.DomainProcess data, CancellationToken token)
        {
            var parameters = data as ReputationData;
            if (parameters == null) return true;
            try
            {
                //var userIds = string.Join(",", parameters.UserIds);
                //TraceLog.WriteInfo($"processing reputation for user {userIds}");
                var proxy = await SignalrClient.GetProxyAsync();
                foreach (var userId in parameters.UserIds)
                {
                    token.ThrowIfCancellationRequested();
                    var command = new UpdateReputationCommand(userId);
                    m_ZboxWriteService.UpdateReputation(command);
                    try
                    {
                        await proxy.Invoke("Score", command.Score, userId);

                    }
                    catch (Exception ex)
                    {
                        TraceLog.WriteError("on signalr reputation", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On update reputation model:" + parameters, ex);
                return false;
            }
            return true;
        }
    }
}
