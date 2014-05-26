using System;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Net;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Cloudents.Mvc4WebRole
{
    public class WebRole : RoleEntryPoint
    {
        //const string webApplicationProjectName = "Web";

        
        public override bool OnStart()
        {


            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
            //ConfigureDiagnostics();

           // ConfigureStorageAccount();
            
            RoleEnvironment.Changed += (s, e) => RoleEnvironment.RequestRecycle();

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
        private bool ValidateRemoteCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors policyErrors)
        {
            bool result = cert.Subject.ToUpper().Contains("CLOUDENTS");

            return result;
        }
        public override void Run()
        {
            try
            {
                var localuri = new Uri(string.Format("https://{0}/", RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["Endpoint2"].IPEndpoint));
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    ServicePointManager.ServerCertificateValidationCallback += ValidateRemoteCertificate;
                    while (true)
                    {
                        try
                        {
                            var request = (HttpWebRequest)WebRequest.Create(localuri);
                            request.Method = "GET";
                            using (request.GetResponse())
                            {
                            }
                            
                            break;
                        }
// ReSharper disable once EmptyGeneralCatchClause
                        catch
                        {
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
