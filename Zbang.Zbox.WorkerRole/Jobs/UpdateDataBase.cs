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
                    Thread.Sleep(TimeSpan.FromHours(1));
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
