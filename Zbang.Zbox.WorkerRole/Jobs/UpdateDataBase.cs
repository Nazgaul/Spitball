using System;
using System.Threading;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.WorkerRole.Jobs
{
    public class UpdateDataBase : IJob
    {
        private readonly IZboxWriteService m_ZboxService;
        private bool m_KeepRunning;
        private int paging = 100;

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
                    var shouldRun = m_ZboxService.Dbi(paging);
                    if (!shouldRun)
                    {
                        Thread.Sleep(TimeSpan.FromDays(1));
                    }
                    paging = paging * 2;
                }
                catch (Exception ex)
                {
                    paging = paging / 2;
                    TraceLog.WriteError("Update Dbi", ex);
                    //   Thread.Sleep(TimeSpan.FromHours(1));
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
