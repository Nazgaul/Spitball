using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource m_CancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent m_RunCompleteEvent = new ManualResetEvent(false);

        private readonly IocFactory m_Unity;

        public WorkerRole()
        {
            m_Unity = new IocFactory();
        }
        public override void Run()
        {
            Trace.TraceInformation("Zbang.Zbox.WorkerRoleSearch is running");
            try
            {
                RunAsync(m_CancellationTokenSource.Token).Wait();
            }
            finally
            {
                m_RunCompleteEvent.Set();
            }
        }

        void RoleEnvironment_StatusCheck(object sender, RoleInstanceStatusCheckEventArgs e)
        {
            if (e.Status == RoleInstanceStatus.Busy)
            {
                TraceLog.WriteError("Status is busy");
            }

        }


        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            RoleEnvironment.Changing += RoleEnvironmentChanging;
            RoleEnvironment.StatusCheck += RoleEnvironment_StatusCheck;
            Trace.TraceInformation("Zbang.Zbox.WorkerRoleSearch has been started");

            return result;
        }

        private void RoleEnvironmentChanging(object sender, RoleEnvironmentChangingEventArgs e)
        {
            TraceLog.WriteWarning("change role Environment");
            RoleEnvironment.RequestRecycle();
        }

        public override void OnStop()
        {
            Trace.TraceInformation("Zbang.Zbox.WorkerRoleSearch is stopping");

            m_CancellationTokenSource.Cancel();
            var jobs = GetJob();
            foreach (var job in jobs)
            {
                job.Stop();
            }
            m_RunCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("Zbang.Zbox.WorkerRoleSearch has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {

            var jobs = GetJob();
            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    Trace.TraceInformation("Working");
                    var list = jobs.Select(job => job.RunAsync(cancellationToken)).ToList();
                    await Task.WhenAll(list);
                    await Task.Delay(1000, cancellationToken);
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                }
            }
        }

        private IList<IJob> GetJob()
        {
            if (RoleEnvironment.IsEmulated)
            {
                return new List<IJob>
                {
                   // m_Unity.Resolve<IJob>(IocFactory.UpdateSearchItem),
                    m_Unity.Resolve<IJob>(IocFactory.UpdateSearchBox),
                   //m_Unity.Resolve<IJob>(IocFactory.UpdateSearchQuiz),
                   // m_Unity.Resolve<IJob>(IocFactory.UpdateSearchUniversity),
                   //m_Unity.Resolve<IJob>(nameof(SchdulerListener))
                   //m_Unity.Resolve<IJob>(nameof(UpdateUnsubscribeList))
                   
                };
            }
            return new List<IJob>
            {
                m_Unity.Resolve<IJob>(IocFactory.UpdateSearchItem),
                m_Unity.Resolve<IJob>(IocFactory.UpdateSearchBox),
                m_Unity.Resolve<IJob>(IocFactory.UpdateSearchQuiz),
                m_Unity.Resolve<IJob>(IocFactory.UpdateSearchUniversity),
                //m_Unity.Resolve<IJob>(nameof(SchdulerListener)),
                m_Unity.Resolve<IJob>(nameof(UpdateUnsubscribeList))
            };
        }
    }
}
