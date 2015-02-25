using System;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRole.DomainProcess
{
    public class DeleteBox : IDomainProcess
    {
        private readonly IZboxWriteService m_ZboxWriteService;

        public DeleteBox(IZboxWriteService zboxWriteService)
        {
            m_ZboxWriteService = zboxWriteService;
        }

        public async Task<bool> ExecuteAsync(Infrastructure.Transport.DomainProcess data)
        {
            var parameters = data as DeleteBoxData;
            if (parameters == null) return true;
            try
            {
                await m_ZboxWriteService.DeleteBoxAsync(new DeleteBoxCommand(parameters.BoxId));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On delete box model:" + parameters, ex);
            }
            return true;
        }
    }
}
