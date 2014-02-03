using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.WcfRestService
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
            ConfigureDiagnostics();

            ConfigureStorageAccount();
            return base.OnStart();
        }

        private void ConfigureStorageAccount()
        {
            // This code sets up a handler to update CloudStorageAccount instances when their corresponding
            // configuration settings change in the service configuration file.
            CloudStorageAccount.SetConfigurationSettingPublisher((configName, configSetter) =>
            {
                // Provide the configSetter with the initial value
                configSetter(RoleEnvironment.GetConfigurationSettingValue(configName));

                RoleEnvironment.Changed += (sender, arg) =>
                {
                    if (arg.Changes.OfType<RoleEnvironmentConfigurationSettingChange>()
                        .Any((change) => (change.ConfigurationSettingName == configName)))
                    {
                        // The corresponding configuration setting has changed, propagate the value
                        if (!configSetter(RoleEnvironment.GetConfigurationSettingValue(configName)))
                        {
                            // In this case, the change to the storage account credentials in the
                            // service configuration is significant enough that the role needs to be
                            // recycled in order to use the latest settings. (for example, the 
                            // endpoint has changed)
                            RoleEnvironment.RequestRecycle();
                        }
                    }
                };
            });
            //var cloudStorageAccount = CloudStorageAccount.FromConfigurationSetting("StorageConnectionString");
            //CreateBlobStorages(cloudStorageAccount.CreateCloudBlobClient());
            //CreateQueues(cloudStorageAccount.CreateCloudQueueClient());
        }

        //private void CreateBlobStorages(CloudBlobClient BlobClient)
        //{
        //    var container = BlobClient.GetContainerReference(BlobProvider.azureBlobContainer.ToLower());

        //    if (container.CreateIfNotExist())
        //    {
        //        container.SetPermissions(new BlobContainerPermissions
        //                                     {
        //            PublicAccess = BlobContainerPublicAccessType.Off
        //        });
        //    }
        //    container = BlobClient.GetContainerReference(BlobProvider.azureCacheContainer.ToLower());
        //    if (container.CreateIfNotExist())
        //    {
        //        container.SetPermissions(new BlobContainerPermissions
        //                                     {
        //            PublicAccess = BlobContainerPublicAccessType.Off
        //        });
        //    }

        //    container = BlobClient.GetContainerReference(BlobProvider.azureProfilePicContainer.ToLower());
        //    if (container.CreateIfNotExist())
        //    {
        //        container.SetPermissions(new BlobContainerPermissions
        //                                     {
        //            PublicAccess = BlobContainerPublicAccessType.Blob
        //        });
        //    }
        //    container = BlobClient.GetContainerReference(BlobProvider.azureThumbnailContainer.ToLower());
        //    if (container.CreateIfNotExist())
        //    {
        //        container.SetPermissions(new BlobContainerPermissions
        //                                     {
        //            PublicAccess = BlobContainerPublicAccessType.Blob
        //        });
        //    }
        //}

        //private void CreateQueues(CloudQueueClient QueueClient)
        //{
        //    var mailQueue = QueueClient.GetQueueReference(QueueProvider.mailQueueName.ToLower());

        //    var queue = QueueClient.GetQueueReference(QueueProvider.queueName.ToLower());

        //    var ThumbanilQueue = QueueClient.GetQueueReference(QueueProvider.thumbnailQueueName.ToLower());
        //    mailQueue.CreateIfNotExist();
        //    queue.CreateIfNotExist();
        //    ThumbanilQueue.CreateIfNotExist();
        //}

        private void ConfigureDiagnostics()
        {
            //TimeSpan scheduledTransferPeriod = TimeSpan.FromMinutes(5);
            //TimeSpan sampleRate = TimeSpan.FromSeconds(30);

            DiagnosticMonitorConfiguration diagnosticMonitorConfiguration = DiagnosticMonitor.GetDefaultInitialConfiguration();
            diagnosticMonitorConfiguration.ConfigurationChangePollInterval = TimeSpan.FromMinutes(5);

            //Windows Event Logs
            diagnosticMonitorConfiguration.WindowsEventLog.DataSources.Add("System!*");
            diagnosticMonitorConfiguration.WindowsEventLog.DataSources.Add("Application!*");
            diagnosticMonitorConfiguration.WindowsEventLog.DataSources.Add("UserData!*");
            diagnosticMonitorConfiguration.WindowsEventLog.ScheduledTransferLogLevelFilter = LogLevel.Critical | LogLevel.Error | LogLevel.Warning;



            //Azure Application Logs
            diagnosticMonitorConfiguration.Logs.ScheduledTransferLogLevelFilter = LogLevel.Verbose;
            diagnosticMonitorConfiguration.Logs.ScheduledTransferPeriod = TimeSpan.FromMinutes(5);
            //diagnosticMonitorConfiguration.Logs.BufferQuotaInMB = 5;

            var counters = new List<string> 
            {
                @"\ASP.NET Applications(__Total__)\Requests/Sec",
                @"\ASP.NET v4.0.30319\Requests Queued",
                @"\ASP.NET v4.0.30319\Request Wait Time",
                @"\ASP.NET v4.0.30319\Requests Rejected",
                @"\ASP.NET v4.0.30319\Request Execution Time",
                @"\Processor(_Total)\% Processor Time",
                @"\Memort\Available MBytes",
                @"\ASP.NET\Application Restarts"
            };
            diagnosticMonitorConfiguration.PerformanceCounters.ScheduledTransferPeriod = TimeSpan.FromMinutes(5);
            counters.ForEach(counter => diagnosticMonitorConfiguration.PerformanceCounters.DataSources.Add(
                new PerformanceCounterConfiguration
                {
                    CounterSpecifier = counter,
                    SampleRate = TimeSpan.FromMinutes(1)
                })
            );

            DiagnosticMonitor.Start("Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString", diagnosticMonitorConfiguration);
        }
    }
}