using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRole.DomainProcess
{
    public class UpdateQuota : IDomainProcess
    {
        private readonly IZboxWriteService m_ZboxWriteService;

        public UpdateQuota(IZboxWriteService zboxWriteService)
        {
            m_ZboxWriteService = zboxWriteService;
        }

        public bool Execute(Infrastructure.Transport.DomainProcess data)
        {
            var parameters = data as QuotaData;
            if (parameters == null) return true;
            try
            {
                m_ZboxWriteService.UpdateQuota(new UpdateQuotaCommand(parameters.UserIds));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On new update model:" + parameters, ex);
            }
            return true;
        }
    }
}
