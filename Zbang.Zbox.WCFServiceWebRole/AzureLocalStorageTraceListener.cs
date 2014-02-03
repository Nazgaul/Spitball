using System;
using System.Diagnostics;
using System.IO;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace Zbang.Zbox.WCFServiceWebRole
{
    public class AzureLocalStorageTraceListener: XmlWriterTraceListener
    {
        //Ctor
        public AzureLocalStorageTraceListener(): base(Path.Combine(AzureLocalStorageTraceListener.GetLogDirectory().Path, "Zbang.Zbox.WCFServiceWebRole.svclog"))
        {
        }

        //Methods
        public static DirectoryConfiguration GetLogDirectory()
        {
            DirectoryConfiguration directory = new DirectoryConfiguration();
            directory.Container = "wad-tracefiles";
            directory.DirectoryQuotaInMB = 10;
            directory.Path = RoleEnvironment.GetLocalResource("Zbang.Zbox.WCFServiceWebRole.svclog").RootPath;
            return directory;
        }
    }
}
