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
    public class UpdateQuota : IDomainProcess
    {
        private readonly IZboxWorkerRoleService m_ZboxWriteService;
        private readonly ILogger m_Logger;

        public UpdateQuota(IZboxWorkerRoleService zboxWriteService, ILogger logger)
        {
            m_ZboxWriteService = zboxWriteService;
            m_Logger = logger;
        }

        public Task<bool> ExecuteAsync(Infrastructure.Transport.DomainProcess data, CancellationToken token)
        {
            if (!(data is QuotaData parameters)) return Infrastructure.Extensions.TaskExtensions.CompletedTaskTrue;
            try
            {
                m_ZboxWriteService.UpdateQuota(new UpdateQuotaCommand(parameters.UserIds));
            }
            catch (Exception ex)
            {
                m_Logger.Exception(ex, new Dictionary<string, string>
                {
                    ["model"] = parameters.ToString()
                });
            }
            return Infrastructure.Extensions.TaskExtensions.CompletedTaskTrue;
        }
    }
}
