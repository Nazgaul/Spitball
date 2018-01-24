using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Cloudents.Core.Interfaces;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRoleSearch.DomainProcess
{
    public class UpdateQuota : IDomainProcess
    {
        private readonly IZboxWorkerRoleService m_ZboxWriteService;
        private readonly ILogger _logger;

        public UpdateQuota(IZboxWorkerRoleService zboxWriteService, ILogger logger)
        {
            m_ZboxWriteService = zboxWriteService;
            _logger = logger;
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
                _logger.Exception(ex, new Dictionary<string, string>
                {
                    ["model"] = parameters.ToString()
                });
            }
            return Infrastructure.Extensions.TaskExtensions.CompletedTaskTrue;
        }
    }
}
