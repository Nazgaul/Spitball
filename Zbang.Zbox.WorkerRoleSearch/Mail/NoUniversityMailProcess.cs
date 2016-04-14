using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.ReadServices;

namespace Zbang.Zbox.WorkerRoleSearch.Mail
{
    public class NoUniversityMailProcess : IMailProcess
    {
        private readonly IZboxReadServiceWorkerRole m_ZboxReadService;

        public NoUniversityMailProcess(IZboxReadServiceWorkerRole zboxReadService)
        {
            m_ZboxReadService = zboxReadService;
        }

        public Task<bool> ExcecuteAsync()
        {
            throw new NotImplementedException();
        }
    }
}
