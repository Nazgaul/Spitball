using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using log4net.Config;
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


        private readonly IList<IJob> m_Jobs;
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

        static void RoleEnvironment_StatusCheck(object sender, RoleInstanceStatusCheckEventArgs e)
        {
            if (e.Status == RoleInstanceStatus.Busy)
            {
                TraceLog.WriteError("Status is busy");
            }

        }


        public override bool OnStart()
        {

            TelemetryConfiguration.Active.InstrumentationKey = RoleEnvironment.GetConfigurationSettingValue("APPINSIGHTS_INSTRUMENTATIONKEY");
            
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            var result = base.OnStart();
            XmlConfigurator.Configure();
            RoleEnvironment.Changing += RoleEnvironmentChanging;
            RoleEnvironment.StatusCheck += RoleEnvironment_StatusCheck;
            Trace.TraceInformation("Zbang.Zbox.WorkerRoleSearch has been started");

            return result;
        }

        private static void RoleEnvironmentChanging(object sender, RoleEnvironmentChangingEventArgs e)
        {
            TraceLog.WriteWarning("change role Environment");
            RoleEnvironment.RequestRecycle();
        }

        public override void OnStop()
        {
            Trace.TraceInformation("Zbang.Zbox.WorkerRoleSearch is stopping");
            m_CancellationTokenSource.Cancel();
            m_RunCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("Zbang.Zbox.WorkerRoleSearch has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            foreach (var job in m_Jobs)
            {
                var t =  Task.Run(() => job.RunAsync(cancellationToken), cancellationToken);
                m_Tasks.Add(t);
            }
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    for (int i = 0; i < m_Tasks.Count; i++)
                    {
                        var task = m_Tasks[i];
                        if (task.IsFaulted)
                        {

                            // Observe unhandled exception
                            if (task.Exception != null)
                            {
                                TraceLog.WriteError("Job threw an exception: ", task.Exception.InnerException);
                            }
                            else
                            {
                                TraceLog.WriteWarning("Job Failed and no exception thrown.");
                            }

                            var jobToRestart = m_Jobs.ElementAt(i);
                            m_Tasks[i] = Task.Run(() => jobToRestart.RunAsync(cancellationToken), cancellationToken);
                        }
                        if (task.IsCompleted)
                        {
                            TraceLog.WriteWarning($"Job finished index: {i}");
                           // m_Jobs.RemoveAt(i);

                        }
                    }
                    await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken).ConfigureAwait(false);
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
                    //m_Unity.Resolve<IJob>(IocFactory.UpdateSearchItem),
                    //m_Unity.Resolve<IJob>(IocFactory.UpdateSearchBox),
                    //m_Unity.Resolve<IJob>(IocFactory.UpdateSearchQuiz),
                    //m_Unity.Resolve<IJob>(IocFactory.UpdateSearchUniversity),
                    //m_Unity.Resolve<IJob>(IocFactory.UpdateSearchFlashcard),
                    //m_Unity.Resolve<IJob>(nameof(UpdateUnsubscribeList)),
                    //m_Unity.Resolve<IJob>(nameof(SchedulerListener)),
                    //m_Unity.Resolve<IJob>(nameof(MailQueueProcess)),
                    //m_Unity.Resolve<IJob>(nameof(TransactionQueueProcess)),
                    //m_Unity.Resolve<IJob>(nameof(ThumbnailQueueProcess)),
                    //m_Unity.Resolve<IJob>(nameof(DeleteOldConnections)),
                    //m_Unity.Resolve<IJob>(nameof(UpdateSearchFeed)),
                    //m_Unity.Resolve<IJob>(nameof(TestingJob))
                    m_Unity.Resolve<IJob>(nameof(Crawler))

                };
            }
            return new List<IJob>
            {
                m_Unity.Resolve<IJob>(IocFactory.UpdateSearchItem), //0
                m_Unity.Resolve<IJob>(IocFactory.UpdateSearchBox), //1
                m_Unity.Resolve<IJob>(IocFactory.UpdateSearchQuiz), //2
                m_Unity.Resolve<IJob>(IocFactory.UpdateSearchUniversity), //3
                m_Unity.Resolve<IJob>(IocFactory.UpdateSearchFlashcard), //4
                m_Unity.Resolve<IJob>(nameof(UpdateUnsubscribeList)), //5
                m_Unity.Resolve<IJob>(nameof(SchedulerListener)), //6
                m_Unity.Resolve<IJob>(nameof(MailQueueProcess)), //7
                m_Unity.Resolve<IJob>(nameof(TransactionQueueProcess)), //8
                m_Unity.Resolve<IJob>(nameof(ThumbnailQueueProcess)), //9
                m_Unity.Resolve<IJob>(nameof(DeleteOldConnections)), //10
                m_Unity.Resolve<IJob>(nameof(UpdateSearchFeed)), //11
                m_Unity.Resolve<IJob>(nameof(Crawler)) //12
               // m_Unity.Resolve<IJob>(nameof(TestingJob)) //13
            };
        }
    }
}
