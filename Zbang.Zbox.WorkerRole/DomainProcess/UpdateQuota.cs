using System;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRole.DomainProcess
{
    public class UpdateQuota : IDomainProcess
    {
        private readonly IZboxWorkerRoleService m_ZboxWriteService;

        public UpdateQuota(IZboxWorkerRoleService zboxWriteService)
        {
            m_ZboxWriteService = zboxWriteService;
        }

        public Task<bool> ExecuteAsync(Infrastructure.Transport.DomainProcess data)
        {
            var parameters = data as QuotaData;
            if (parameters == null) return Infrastructure.Extensions.TaskExtensions.CompletedTaskTrue;
            try
            {
                m_ZboxWriteService.UpdateQuota(new UpdateQuotaCommand(parameters.UserIds));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On new update model:" + parameters, ex);
            }
            return Infrastructure.Extensions.TaskExtensions.CompletedTaskTrue;
        }
    }
}
