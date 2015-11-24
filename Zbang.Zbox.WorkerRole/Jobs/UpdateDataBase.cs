using System;
using System.Diagnostics;
using System.Threading;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.WorkerRole.Jobs
{
    public class UpdateDataBase : IJob
    {
        private readonly IZboxWorkerRoleService m_ZboxService;
        private bool m_KeepRunning;

        public UpdateDataBase(IZboxWorkerRoleService zboxService)
        {
            m_ZboxService = zboxService;
        }

        public void Run()
        {
            int index = 0;
            m_KeepRunning = true;
            while (m_KeepRunning)
            {
                var sw = new Stopwatch();
                TraceLog.WriteInfo("Running update database");
                try
                {
                    sw.Restart();
                    if (!m_ZboxService.Dbi(index))
                    {
                        sw.Stop();
                        TraceLog.WriteInfo("Running update database took(milliseconds): " + sw.ElapsedMilliseconds);
                        index = 0;
                        Thread.Sleep(TimeSpan.FromHours(2));
                    }
                    index++;
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
}
