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
    public class DeleteBox : IDomainProcess
    {
        private readonly IZboxWorkerRoleService _zboxWriteService;
        private readonly ILogger _logger;

        public DeleteBox(IZboxWorkerRoleService zboxWriteService, ILogger logger)
        {
            _zboxWriteService = zboxWriteService;
            _logger = logger;
        }

        public async Task<bool> ExecuteAsync(Infrastructure.Transport.DomainProcess data, CancellationToken token)
        {
            if (!(data is DeleteBoxData parameters)) return true;
            try
            {
                await _zboxWriteService.DeleteBoxAsync(new DeleteBoxCommand(parameters.BoxId)).ConfigureAwait(false);
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
