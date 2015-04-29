using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Cloudents.OneTimeWorkerRole
{
    public class OneTimeDbi : IOneTimeDbi
    {
        private readonly IZboxWorkerRoleService m_ZboxService;

        public OneTimeDbi(IZboxWorkerRoleService zboxService)
        {
            m_ZboxService = zboxService;
        }

        public void Run()
        {
            try
            {
                m_ZboxService.OneTimeDbi();
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On onetime dbi", ex);
            }
        }
    }

    public interface IOneTimeDbi
    {
        void Run();
    }
}
