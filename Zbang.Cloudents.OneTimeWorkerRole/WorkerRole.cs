using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;

namespace Zbang.Cloudents.OneTimeWorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
       private  readonly UnityFactory m_Unity;
       public WorkerRole()
       {
           m_Unity = new UnityFactory();
       }
        public override void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            Trace.TraceInformation("Zbang.Cloudents.OneTimeWorkerRole entry point called", "Information");
//            var updateThumbnail = m_Unity.Resolve<IUpdateThumbnails>();
          //  updateThumbnail.UpdateThumbnailPicture();
            while (true)
            {
                Thread.Sleep(10000);
                Trace.TraceInformation("Working", "Information");
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;
           
            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }
    }
}
