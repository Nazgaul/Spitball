using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus;

namespace Zbang.Zbox.Infrastructure.Notifications
{
    public class QueueProvider
    {
        public void Initialize()
        {
            ServiceBusEnvironment.SystemConnectivity.Mode = ConnectivityMode.Http;
            NamespaceManager x =
                new NamespaceManager(
                    "Endpoint=sb://cloudentsmsg-ns.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=oePM1T/GBe2ZlaDhik3MLHNXstsM4lhnCTyRTBi0bmQ=");
            
        }
    }
}
