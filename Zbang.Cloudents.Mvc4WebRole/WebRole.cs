using System;
using System.Linq;
using Microsoft.Web.Administration;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Net;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Cloudents.Mvc4WebRole
{
// ReSharper disable once UnusedMember.Global
    public class WebRole : RoleEntryPoint
    {
        //const string webApplicationProjectName = "Web";

        
        public override bool OnStart()
        {
            //http://fabriccontroller.net/blog/posts/iis-8-0-application-initialization-module-in-a-windows-azure-web-role/
            //using (var serverManager = new ServerManager())
            //{
            //    var mainSite = serverManager.Sites[RoleEnvironment.CurrentRoleInstance.Id + "_Web"];
            //    var mainApplication = mainSite.Applications["/"];
            //    mainApplication["preloadEnabled"] = true;

            //    var mainApplicationPool = serverManager.ApplicationPools[mainApplication.ApplicationPoolName];
            //    mainApplicationPool["startMode"] = "AlwaysRunning";

            //    serverManager.CommitChanges();
            //}

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
            //ConfigureDiagnostics();

           // ConfigureStorageAccount();
            
            //RoleEnvironment.Changed += (s, e) =>
            //{
            //    TraceLog.WriteWarning("recycling machine");
            //    RoleEnvironment.RequestRecycle();
            //};
            //RoleEnvironment.Changing += RoleEnvironment_Changing;

            //RoleEnvironment.Changing += (s, e) =>
            //{


            //    if (e.Changes.Any(change => change is RoleEnvironmentTopologyChange))
            //    {

            //        Trace.WriteLine("Topology changed");
            //        e.Cancel = false;
            //        return;
            //    }
            //    var changes = e.Changes.OfType<RoleEnvironmentConfigurationSettingChange>();
            //    foreach (var item in changes)
            //    {
            //        Trace.WriteLine(item.ConfigurationSettingName);
            //    }
            //    if (changes.First().ConfigurationSettingName == "CdnEndpoint")
            //    {
            //        Trace.WriteLine("Cdn enpoint changed");
            //        using (ServerManager serverManager = new ServerManager())
            //        {
            //            try
            //            {
            //                var appPoolName = serverManager.Sites[RoleEnvironment.CurrentRoleInstance.Id + "_" + webApplicationProjectName].Applications.First().ApplicationPoolName;
            //                var appPool = serverManager.ApplicationPools[appPoolName];
            //                appPool.Recycle();
            //            }
            //            catch (Exception ex)
            //            {
            //                Trace.WriteLine(ex);
            //                e.Cancel = true;
            //                return;
            //            }
            //        }
            //        e.Cancel = false;
            //    }
            //};

            //            private void RoleEnvironmentChanging(object sender, RoleEnvironmentChangingEventArgs e) 
            //{
            //   // Implements the changes after restarting the role instance
            //   if ((e.Changes.Any(change => change is RoleEnvironmentConfigurationSettingChange))) 
            //   {
            //      e.Cancel = true;
            //   }
            //}




            return base.OnStart();
        }

        void RoleEnvironment_Changing(object sender, RoleEnvironmentChangingEventArgs e)
        {
            //var configurationChanges = e.Changes
            //                            .OfType<RoleEnvironmentConfigurationSettingChange>()
            //                            .ToList();

            //if (!configurationChanges.Any()) return;

            //if (configurationChanges.Any(c => c.ConfigurationSettingName == "StorageAccount"))
            //    e.Cancel = true;
        }

        private bool ValidateRemoteCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors policyErrors)
        {
            var result = cert.Subject.ToUpper().Contains("SPITBALL");

            return result;
        }
        public override void Run()
        {
            using (var serverManager = new ServerManager())
            {
                var mainSite = serverManager.Sites[RoleEnvironment.CurrentRoleInstance.Id + "_Web"];
                var mainApplication = mainSite.Applications["/"];
                mainApplication["preloadEnabled"] = true;

                var mainApplicationPool = serverManager.ApplicationPools[mainApplication.ApplicationPoolName];
                mainApplicationPool["startMode"] = "AlwaysRunning";

                serverManager.CommitChanges();
            }

            try
            {

                var localUri = new Uri(
                    $"https://{RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["Endpoint2"].IPEndpoint}/");
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    ServicePointManager.ServerCertificateValidationCallback += ValidateRemoteCertificate;
                    while (true)
                    {
                        try
                        {
                            var request = (HttpWebRequest)WebRequest.Create(localUri);
                            request.Method = "GET";
                            using (request.GetResponse())
                            {
                            }
                            TraceLog.WriteInfo("breaking the on run task");
                            break;
                        }
                        catch(Exception ex)
                        {
                            TraceLog.WriteError("Run", ex);
                        }
                        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(10));
                    }
                });
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("on Run", ex);
            }
            base.Run();
        }

        //private void ConfigureStorageAccount()
        //{
            // This code sets up a handler to update CloudStorageAccount instances when their corresponding
            // configuration settings change in the service configuration file.
            //RoleEnvironment.Changed += (s, e) =>
            //{
            //    RoleEnvironment.RequestRecycle();
            //};
            //CloudStorageAccount.SetConfigurationSettingPublisher((configName, configSetter) =>
            //{
            //    // Provide the configSetter with the initial value
            //    configSetter(RoleEnvironment.GetConfigurationSettingValue(configName));

            //    RoleEnvironment.Changed += (sender, arg) =>
            //    {
            //        if (arg.Changes.OfType<RoleEnvironmentConfigurationSettingChange>()
            //            .Any((change) => (change.ConfigurationSettingName == configName)))
            //        {
            //            // The corresponding configuration setting has changed, propagate the value
            //            if (!configSetter(RoleEnvironment.GetConfigurationSettingValue(configName)))
            //            {
            //                // In this case, the change to the storage account credentials in the
            //                // service configuration is significant enough that the role needs to be
            //                // recycled in order to use the latest settings. (for example, the 
            //                // endpoint has changed)
            //                RoleEnvironment.RequestRecycle();
            //            }
            //        }
            //    };
            //});
            //var cloudStorageAccount = CloudStorageAccount.FromConfigurationSetting("StorageConnectionString");
            //CreateBlobStorages(cloudStorageAccount.CreateCloudBlobClient());
            //CreateQueues(cloudStorageAccount.CreateCloudQueueClient());
       // }


    }
}
