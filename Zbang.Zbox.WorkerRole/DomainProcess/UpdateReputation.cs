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
    public class UpdateReputation : IDomainProcess
    {
        private readonly IZboxWriteService m_ZboxWriteService;

        public UpdateReputation(IZboxWriteService zboxWriteService)
        {
            m_ZboxWriteService = zboxWriteService;
        }

        public bool Execute(Infrastructure.Transport.DomainProcess data)
        {
            var parameters = data as ReputationData;
            if (parameters == null) return true;
            try
            {
                m_ZboxWriteService.UpdateReputation(new UpdateReputationCommand(parameters.UserId));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On new update model:" + parameters, ex);
            }
            return true;
        }
    }
}
