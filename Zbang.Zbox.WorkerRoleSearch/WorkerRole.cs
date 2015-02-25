using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource m_CancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent m_RunCompleteEvent = new ManualResetEvent(false);

        readonly UnityFactory m_Unity;

        public WorkerRole()
        {
            m_Unity = new UnityFactory();
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


        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            RoleEnvironment.Changing += RoleEnvironmentChanging;
            Trace.TraceInformation("Zbang.Zbox.WorkerRoleSearch has been started");

            return result;
        }

        private void RoleEnvironmentChanging(object sender, RoleEnvironmentChangingEventArgs e)
        {
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
            var job = m_Unity.Resolve<IJob>(UnityFactory.UpdateSearch);
            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");
                job.Run();
                await Task.Delay(1000);
            }
        }
    }
}
