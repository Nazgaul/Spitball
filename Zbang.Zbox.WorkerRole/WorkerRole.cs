using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.WorkerRole.Jobs;
using Zbang.Zbox.WorkerRole.OneTimeUpdates;
using Zbang.Zbox.Infrastructure.Ioc;
//using Zbang.Zbox.WorkerRole.OneTimeUpdates;

namespace Zbang.Zbox.WorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {

        private readonly IEnumerable<IJob> m_Jobs;
        private readonly List<Task> m_Tasks;
        private bool m_KeepRunning;
        readonly UnityFactory m_Unity;

        public WorkerRole()
        {
            try
            {
                m_Unity = new UnityFactory();
#if DEBUG
                log4net.Config.XmlConfigurator.Configure();
#endif
                m_Tasks = new List<Task>();
                m_Jobs = CreateJobProcessors();
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("ON WORKER ROLE CTOR ", ex);
            }
        }

        private IEnumerable<IJob> CreateJobProcessors()
        {

            return new[]
                       {
                           m_Unity.Unity.Resolve<IJob>(UnityFactory.DigestEmail2,new IocParameterOverride("hourForEmailDigest",NotificationSettings.OnceADay))
                           //m_Unity.Unity.Resolve<IJob>(UnityFactory.DigestEmail2,new IocParameterOverride("hourForEmailDigest",NotificationSettings.OnEveryChange)),
                           //m_Unity.Unity.Resolve<IJob>(UnityFactory.DigestEmail2,new IocParameterOverride("hourForEmailDigest",NotificationSettings.OnceAWeek)),
                           //////m_Unity.Resolve<IJob>(UnityFactory.DeleteCahceBlobContainer),
                           //m_Unity.Resolve<IJob>(UnityFactory.GenerateDocumentCache),
                           //m_Unity.Resolve<IJob>(UnityFactory.AddFiles),
                           //m_Unity.Resolve<IJob>(UnityFactory.Transaction)
                           //m_Unity.Resolve<IJob>(UnityFactory.Dbi),
                           //m_Unity.Unity.Resolve<IJob>(UnityFactory.MailProcess2)
                       };
        }
        public override void Run()
        {

            try
            {
                m_KeepRunning = true;

                // Start the jobs
                foreach (var job in m_Jobs)
                {
                    var t = Task.Factory.StartNew(job.Run);
                    m_Tasks.Add(t);
                }

                // Control and restart a faulted job
                while (m_KeepRunning)
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
                                TraceLog.WriteInfo("Job Failed and no exception thrown.");
                            }

                            var jobToRestart = m_Jobs.ElementAt(i);
                            m_Tasks[i] = Task.Factory.StartNew(jobToRestart.Run);
                        }
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(30));
                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("WORKER ROLE RUN", ex);
            }
        }



        public override bool OnStart()
        {

            RoleEnvironment.Changing += RoleEnvironmentChanging;
            //RoleEnvironment.Changed += RoleEnvironmentChanged;
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }

        public override void OnStop()
        {
            m_KeepRunning = false;

            foreach (var job in m_Jobs)
            {
                job.Stop();
                var disposable = job as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            try
            {
                Task.WaitAll(m_Tasks.ToArray());
            }
            catch (AggregateException ex)
            {
                // Observe any unhandled exceptions.
                TraceLog.WriteInfo(string.Format("Finalizing exception thrown: {0} exceptions", ex.InnerExceptions.Count));
            }

            base.OnStop();
        }

        private static void RoleEnvironmentChanging(object sender, RoleEnvironmentChangingEventArgs e)
        {
            if (e.Changes.Any(change => change is RoleEnvironmentConfigurationSettingChange))
            {
                e.Cancel = true;
            }
        }

        //private static void RoleEnvironmentChanged(object sender, RoleEnvironmentChangedEventArgs e)
        //{
        //    // configure trace listener for any changes to EnableTableStorageTraceListener 
        //    if (e.Changes.OfType<RoleEnvironmentConfigurationSettingChange>().Any(change => change.ConfigurationSettingName == "TraceEventTypeFilter"))
        //    {
        //        ConfigureTraceListener(RoleEnvironment.GetConfigurationSettingValue("TraceEventTypeFilter"));
        //    }
        //}
    }
}
