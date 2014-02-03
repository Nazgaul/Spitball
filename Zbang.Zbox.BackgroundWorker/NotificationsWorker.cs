using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Infrastructure.WorkerRoles;
using System.Threading;
using System.Diagnostics;

namespace Zbang.Zbox.BackgroundWorker
{
    internal class NotificationsWorker: WorkerEntryPoint
    {
        //Fields
        private int SLEEP_TIME = 1000;

        //Methods
        public override void Run()
        {
            while (true)
            {
                //TODO: Process Notifications and Rules
                //      Notify Periodically

                Trace.TraceInformation("Notifications Worker doing some job...");

                Thread.Sleep(SLEEP_TIME);
            }
        }
    }
}
