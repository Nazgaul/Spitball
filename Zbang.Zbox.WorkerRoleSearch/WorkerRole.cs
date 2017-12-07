using System;
using System.Collections.Generic;
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
        private readonly CancellationTokenSource _mCancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent _mRunCompleteEvent = new ManualResetEvent(false);

        private readonly IocFactory _mUnity;

        private readonly IList<IJob> _mJobs;
        private readonly List<Task> _mTasks = new List<Task>();
        private readonly ILogger _logger;

        public WorkerRole()
        {
            _mUnity = new IocFactory();
            _mJobs = GetJob();
            _logger = _mUnity.Resolve<ILogger>();
        }

        public override void Run()
        {
            _logger.Info("Zbang.Zbox.WorkerRoleSearch is running");
            try
            {
                RunAsync(_mCancellationTokenSource.Token).Wait();
            }
            finally
            {
                _mRunCompleteEvent.Set();
            }
        }

        private void RoleEnvironment_StatusCheck(object sender, RoleInstanceStatusCheckEventArgs e)
        {
            if (e.Status == RoleInstanceStatus.Busy)
            {
                _logger.Error("Status is busy");
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
            _logger.Info("Zbang.Zbox.WorkerRoleSearch has been started");

            return result;
        }

        private void RoleEnvironmentChanging(object sender, RoleEnvironmentChangingEventArgs e)
        {
            _logger.Warning("change role Environment");
            RoleEnvironment.RequestRecycle();
        }

        public override void OnStop()
        {
            _logger.Info("Zbang.Zbox.WorkerRoleSearch is stopping");
            _mCancellationTokenSource.Cancel();
            _mRunCompleteEvent.WaitOne();

            base.OnStop();
            _logger.Info("Zbang.Zbox.WorkerRoleSearch has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            foreach (var job in _mJobs)
            {
                var t =  Task.Run(() => job.RunAsync(cancellationToken), cancellationToken);
                _mTasks.Add(t);
            }
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    for (int i = 0; i < _mTasks.Count; i++)
                    {
                        var task = _mTasks[i];
                        if (task.IsFaulted)
                        {
                            // Observe unhandled exception
                            if (task.Exception != null)
                            {
                                _logger.Exception(task.Exception.InnerException,
                                    new Dictionary<string, string> {["job"] = i.ToString() });
                            }
                            else
                            {
                                _logger.Error("Job Failed and no exception thrown.");
                            }

                            var jobToRestart = _mJobs[i];
                            _mTasks[i] = Task.Run(() => jobToRestart.RunAsync(cancellationToken), cancellationToken);
                        }
                        if (task.IsCompleted)
                        {
                            _logger.Warning($"Job finished index: {i}");
                            _mTasks.RemoveAt(i);
                        }
                    }
                    await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.Exception(ex);
                }
            }
        }

        private IList<IJob> GetJob()
        {
            if (RoleEnvironment.IsEmulated)
            {
                return new List<IJob>
                {
                    //m_Unity.Resolve<IJob>(IocFactory.UpdateSearchItem), //0
                    //m_Unity.Resolve<IJob>(IocFactory.UpdateSearchBox), //1
                    //m_Unity.Resolve<IJob>(IocFactory.UpdateSearchQuiz), //2
                    _mUnity.Resolve<IJob>(IocFactory.UpdateSearchUniversity), //3
                    //m_Unity.Resolve<IJob>(IocFactory.UpdateSearchFlashcard), //4
                    //m_Unity.Resolve<IJob>(nameof(UpdateUnsubscribeList)), //5
                    //m_Unity.Resolve<IJob>(nameof(SchedulerListener)), //6
                    //m_Unity.Resolve<IJob>(nameof(MailQueueProcess)), //7
                    //m_Unity.Resolve<IJob>(nameof(TransactionQueueProcess)), //8
                    //m_Unity.Resolve<IJob>(nameof(ThumbnailQueueProcess)), //9
                    //m_Unity.Resolve<IJob>(nameof(DeleteOldConnections)), //10
                    //m_Unity.Resolve<IJob>(nameof(UpdateSearchFeed)), //11
                    //m_Unity.Resolve<IJob>(nameof(Crawler)) //12

                   // m_Unity.Resolve<IJob>(nameof(BlobManagement))
                   // _mUnity.Resolve<IJob>(nameof(TestingJob))

                };
            }
            return new List<IJob>
            {
                _mUnity.Resolve<IJob>(IocFactory.UpdateSearchItem), //0
                _mUnity.Resolve<IJob>(IocFactory.UpdateSearchBox), //1
                _mUnity.Resolve<IJob>(IocFactory.UpdateSearchQuiz), //2
                _mUnity.Resolve<IJob>(IocFactory.UpdateSearchUniversity), //3
                _mUnity.Resolve<IJob>(IocFactory.UpdateSearchFlashcard), //4
                _mUnity.Resolve<IJob>(nameof(UpdateUnsubscribeList)), //5
                _mUnity.Resolve<IJob>(nameof(SchedulerListener)), //6
                _mUnity.Resolve<IJob>(nameof(MailQueueProcess)), //7
                _mUnity.Resolve<IJob>(nameof(TransactionQueueProcess)), //8
                _mUnity.Resolve<IJob>(nameof(ThumbnailQueueProcess)), //9
                _mUnity.Resolve<IJob>(nameof(DeleteOldConnections)), //10
                _mUnity.Resolve<IJob>(nameof(UpdateSearchFeed)), //11
                //m_Unity.Resolve<IJob>(nameof(BlobManagement))
                //m_Unity.Resolve<IJob>(nameof(Crawler)) //12
               // m_Unity.Resolve<IJob>(nameof(TestingJob)) //13
            };
        }
    }
}
