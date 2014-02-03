using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using Zbang.Zbox.Infrastructure.WorkerRoles;

namespace Zbang.Zbox.BackgroundWorker
{
    public class WorkerRole: ThreadedRoleEntryPoint
    {
        public override void Run()
        {            
            Trace.TraceInformation("BackgroundWorker entry point called");

            base.Run();
        }

        public override bool OnStart()
        {
            this.ConfigureDiagnostics();

            this.ConfigureStorageAccount();

            List<WorkerEntryPoint> workers = new List<WorkerEntryPoint>();

            //TODO: Currently do nothing...                        
            workers.Add(new EventsWorker());
            workers.Add(new NotificationsWorker());

            return base.OnStart(workers.ToArray());
        }

        //TODO: Refactor to remove duplications
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
        }

        private void ConfigureDiagnostics()
        {
            TimeSpan scheduledTransferPeriod = TimeSpan.FromMinutes(1);

            DiagnosticMonitorConfiguration diagnosticMonitorConfiguration = DiagnosticMonitor.GetDefaultInitialConfiguration();

            //Windows Event Logs
            diagnosticMonitorConfiguration.WindowsEventLog.DataSources.Add("System!*");
            diagnosticMonitorConfiguration.WindowsEventLog.DataSources.Add("Application!*");
            diagnosticMonitorConfiguration.WindowsEventLog.ScheduledTransferLogLevelFilter = LogLevel.Verbose;
            diagnosticMonitorConfiguration.WindowsEventLog.ScheduledTransferPeriod = scheduledTransferPeriod;

            //?
            diagnosticMonitorConfiguration.Directories.ScheduledTransferPeriod = scheduledTransferPeriod;

            //Azure Application Logs
            diagnosticMonitorConfiguration.Logs.ScheduledTransferLogLevelFilter = LogLevel.Verbose;
            diagnosticMonitorConfiguration.Logs.ScheduledTransferPeriod = scheduledTransferPeriod;

            // Performance counters
            diagnosticMonitorConfiguration.PerformanceCounters.DataSources.Add(new PerformanceCounterConfiguration()
            {
                CounterSpecifier = @"\Processor(_Total)\% Processor Time",
                SampleRate = TimeSpan.FromMinutes(5)
            });

            diagnosticMonitorConfiguration.PerformanceCounters.DataSources.Add(new PerformanceCounterConfiguration()
            {
                CounterSpecifier = @"\Memory\Available Mbytes",
                SampleRate = TimeSpan.FromMinutes(5)
            });

            diagnosticMonitorConfiguration.PerformanceCounters.ScheduledTransferPeriod = scheduledTransferPeriod;

            DiagnosticMonitor.Start("Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString", diagnosticMonitorConfiguration);
        }
    }
}
