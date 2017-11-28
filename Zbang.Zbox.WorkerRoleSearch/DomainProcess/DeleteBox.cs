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
    public class DeleteBox : IDomainProcess
    {
        private readonly IZboxWorkerRoleService m_ZboxWriteService;
        private readonly ILogger _logger;

        public DeleteBox(IZboxWorkerRoleService zboxWriteService, ILogger logger)
        {
            m_ZboxWriteService = zboxWriteService;
            _logger = logger;
        }

        public async Task<bool> ExecuteAsync(Infrastructure.Transport.DomainProcess data, CancellationToken token)
        {
            if (!(data is DeleteBoxData parameters)) return true;
            try
            {
                await m_ZboxWriteService.DeleteBoxAsync(new DeleteBoxCommand(parameters.BoxId)).ConfigureAwait(false);
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
