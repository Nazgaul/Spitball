using System;
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

        public DeleteBox(IZboxWorkerRoleService zboxWriteService)
        {
            m_ZboxWriteService = zboxWriteService;
        }

        public async Task<bool> ExecuteAsync(Infrastructure.Transport.DomainProcess data, CancellationToken token)
        {
            var parameters = data as DeleteBoxData;
            if (parameters == null) return true;
            try
            {
                await m_ZboxWriteService.DeleteBoxAsync(new DeleteBoxCommand(parameters.BoxId)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On delete box model:" + parameters, ex);
                return false;
            }
            return true;
        }
    }
}
