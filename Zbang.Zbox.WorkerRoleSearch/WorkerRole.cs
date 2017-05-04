using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.WindowsAzure.ServiceRuntime;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource m_CancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent m_RunCompleteEvent = new ManualResetEvent(false);

        private readonly IocFactory m_Unity;


        private readonly IEnumerable<IJob> m_Jobs;
        private readonly List<Task> m_Tasks = new List<Task>();

        public WorkerRole()
        {
            m_Unity = new IocFactory();
            m_Jobs = GetJob();

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
            TelemetryConfiguration.Active.InstrumentationKey = "bceed25d-fdd9-4581-b477-068120221ebd";
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
            //var jobs = GetJob();
            //foreach (var job in jobs)
            //{
            //    job.Stop();
            //}
            m_RunCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("Zbang.Zbox.WorkerRoleSearch has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            foreach (var job in m_Jobs)
            {
                var t = Task.Factory.StartNew(async () => await job.RunAsync(cancellationToken).ConfigureAwait(false), cancellationToken);
                m_Tasks.Add(t);
            }
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    for (int i = 0; i < m_Tasks.Count; i++)
                    {
                        var task = m_Tasks[i];
                        if (!task.IsFaulted) continue;
                        // Observe unhandled exception
                        if (task.Exception != null)
                        {
                            TraceLog.WriteError("Job threw an exception: ", task.Exception.InnerException);
                        }
                        else
                        {
                            TraceLog.WriteInfo("Job Failed and no exception thrown.");
                        }

                        var jobToRestart = m_Jobs.ElementAt(i);
                        m_Tasks[i] = Task.Factory.StartNew(async () => await jobToRestart.RunAsync(cancellationToken).ConfigureAwait(false), cancellationToken);
                    }
                    await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                }
            }
        }

        private IEnumerable<IJob> GetJob()
        {
            if (RoleEnvironment.IsEmulated)
            {
                return new List<IJob>
                {
                    //m_Unity.Resolve<IJob>(nameof(MailQueueProcess))
                    m_Unity.Resolve<IJob>(IocFactory.UpdateSearchItem),
                   // m_Unity.Resolve<IJob>(nameof(BlobManagement)),
                    //m_Unity.Resolve<IJob>(IocFactory.UpdateSearchBox),
                   // m_Unity.Resolve<IJob>(IocFactory.UpdateSearchQuiz),
                   // m_Unity.Resolve<IJob>(IocFactory.UpdateSearchUniversity),
                  // m_Unity.Resolve<IJob>(IocFactory.UpdateSearchFlashcard),
                    //m_Unity.Resolve<IJob>(nameof(SchedulerListener))
                    //m_Unity.Resolve<IJob>(nameof(UpdateUnsubscribeList))
                   // m_Unity.Resolve<IJob>(nameof(TestingJob)),
                    // m_Unity.Resolve<IJob>(nameof(ThumbnailQueueProcess)),
                    // m_Unity.Resolve<IJob>(nameof(DeleteOldConnections)),
                   // m_Unity.Resolve<IJob>(nameof(TransactionQueueProcess))
                   //  m_Unity.Resolve<IJob>(nameof(DeleteOldStuff))
                   //m_Unity.Resolve<IJob>(nameof(UpdateSearchFeed))

                };
            }
            return new List<IJob>
            {
                m_Unity.Resolve<IJob>(IocFactory.UpdateSearchItem),
                m_Unity.Resolve<IJob>(IocFactory.UpdateSearchBox),
                m_Unity.Resolve<IJob>(IocFactory.UpdateSearchQuiz),
                m_Unity.Resolve<IJob>(IocFactory.UpdateSearchUniversity),
                m_Unity.Resolve<IJob>(IocFactory.UpdateSearchFlashcard),
                m_Unity.Resolve<IJob>(nameof(UpdateUnsubscribeList)),
                m_Unity.Resolve<IJob>(nameof(SchedulerListener)),
                m_Unity.Resolve<IJob>(nameof(MailQueueProcess)),
                m_Unity.Resolve<IJob>(nameof(TransactionQueueProcess)),
                m_Unity.Resolve<IJob>(nameof(ThumbnailQueueProcess)),
                m_Unity.Resolve<IJob>(nameof(DeleteOldConnections)),
                m_Unity.Resolve<IJob>(nameof(DeleteOldStuff)),
                m_Unity.Resolve<IJob>(nameof(UpdateSearchFeed))

            };
        }
    }
}
