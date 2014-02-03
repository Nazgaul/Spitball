using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.WorkerRole.Jobs;

namespace Zbang.Zbox.WorkerRole.Jobs
{
    public class UpdateDataBase : IJob
    {
        private readonly IZboxWriteService m_ZboxService;
        private bool m_KeepRunning;

        public UpdateDataBase(IZboxWriteService zboxService)
        {
            m_ZboxService = zboxService;
        }

        public void Run()
        {
            m_KeepRunning = true;
            while (m_KeepRunning)
            {
                try
                {
                    m_ZboxService.Dbi();
                    Thread.Sleep(TimeSpan.FromDays(1));
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError("Update Dbi", ex);
                }
            }
        }

        public void Stop()
        {
            m_KeepRunning = false;
        }
    }
    //public interface IUpdateDataBase
    //{
    //    void UpdateDb();
    //}
}
