using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Common;

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
            m_ZboxService.OneTimeDbi();
        }
    }

    public interface IOneTimeDbi
    {
        void Run();
    }
}
