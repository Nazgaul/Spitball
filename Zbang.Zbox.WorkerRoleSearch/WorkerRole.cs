using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.WindowsAzure.ServiceRuntime;
using Cloudents.Core.Interfaces;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent _runCompleteEvent = new ManualResetEvent(false);

        private readonly IocFactory _ioc;

        private readonly IList<IJob> _jobs;
        private readonly List<Task> _tasks = new List<Task>();
        private readonly ILogger _logger;

        public WorkerRole()
        {
            _ioc = new IocFactory();
            _jobs = GetJob();
            _logger = _ioc.Resolve<ILogger>();
        }

        public override void Run()
        {
            _logger.Info("Zbang.Zbox.WorkerRoleSearch is running");
            try
            {
                RunAsync(_cancellationTokenSource.Token).Wait();
            }
            finally
            {
                _runCompleteEvent.Set();
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
            _cancellationTokenSource.Cancel();
            _runCompleteEvent.WaitOne();

            base.OnStop();
            _logger.Info("Zbang.Zbox.WorkerRoleSearch has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            foreach (var job in _jobs)
            {
                var t =  Task.Run(() => job.RunAsync(cancellationToken), cancellationToken);
                _tasks.Add(t);
            }
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    for (int i = 0; i < _tasks.Count; i++)
                    {
                        var task = _tasks[i];
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

                            var jobToRestart = _jobs[i];
                            _tasks[i] = Task.Run(() => jobToRestart.RunAsync(cancellationToken), cancellationToken);
                        }
                        if (task.IsCompleted)
                        {
                            _logger.Warning($"Job finished index: {i}");
                            _tasks.RemoveAt(i);
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
                    _ioc.Resolve<IJob>(nameof(TestingJob))
                    //_ioc.Resolve<IJob>(nameof(SchedulerListener)), //6

                };
            }
            return new List<IJob>
            {
                _ioc.Resolve<IJob>(IocFactory.UpdateSearchItem), //0
                _ioc.Resolve<IJob>(IocFactory.UpdateSearchBox), //1
                _ioc.Resolve<IJob>(IocFactory.UpdateSearchQuiz), //2
                _ioc.Resolve<IJob>(IocFactory.UpdateSearchUniversity), //3
                _ioc.Resolve<IJob>(IocFactory.UpdateSearchFlashcard), //4
                //_ioc.Resolve<IJob>(nameof(UpdateUnsubscribeList)), //5 THIS ONE CAUSE ISSUE
                _ioc.Resolve<IJob>(nameof(SchedulerListener)), //6
                _ioc.Resolve<IJob>(nameof(MailQueueProcess)), //7
                _ioc.Resolve<IJob>(nameof(TransactionQueueProcess)), //8
                _ioc.Resolve<IJob>(nameof(ThumbnailQueueProcess)), //9
                _ioc.Resolve<IJob>(nameof(DeleteOldConnections)), //10
            };
        }
    }
}
