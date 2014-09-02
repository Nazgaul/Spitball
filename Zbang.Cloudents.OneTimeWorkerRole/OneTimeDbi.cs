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
        private readonly IZboxWriteService m_ZboxService;

        public OneTimeDbi(IZboxWriteService zboxService)
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
                Run();
            }
        }
    }

    public interface IOneTimeDbi
    {
        void Run();
    }
}
