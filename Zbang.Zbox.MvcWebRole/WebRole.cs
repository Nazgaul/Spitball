using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace Zbang.Zbox.MvcWebRole
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
            //RoleEnvironment.Changing += RoleEnvironmentChanging;

            this.ConfigureDiagnostics();

            this.ConfigureStorageAccount();

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
        }

        private void ConfigureDiagnostics()
        {
            TimeSpan scheduledTransferPeriod = TimeSpan.FromMinutes(5);
            TimeSpan sampleRate = TimeSpan.FromSeconds(30);

            DiagnosticMonitorConfiguration diagnosticMonitorConfiguration = DiagnosticMonitor.GetDefaultInitialConfiguration();
            //diagnosticMonitorConfiguration.OverallQuotaInMB = 3087;

            //Windows Event Logs
            diagnosticMonitorConfiguration.WindowsEventLog.DataSources.Add("System!*");
            diagnosticMonitorConfiguration.WindowsEventLog.DataSources.Add("Application!*");
            diagnosticMonitorConfiguration.WindowsEventLog.DataSources.Add("UserData!*");
            diagnosticMonitorConfiguration.WindowsEventLog.ScheduledTransferLogLevelFilter = LogLevel.Critical | LogLevel.Error | LogLevel.Warning;
            diagnosticMonitorConfiguration.WindowsEventLog.BufferQuotaInMB = 10;            
            diagnosticMonitorConfiguration.WindowsEventLog.ScheduledTransferPeriod = scheduledTransferPeriod;

            //We dont have directories
            //diagnosticMonitorConfiguration.Directories.ScheduledTransferPeriod = scheduledTransferPeriod;

            //Azure Application Logs
            diagnosticMonitorConfiguration.Logs.ScheduledTransferLogLevelFilter = LogLevel.Critical | LogLevel.Error;
            diagnosticMonitorConfiguration.Logs.BufferQuotaInMB = 5;            
            diagnosticMonitorConfiguration.Logs.ScheduledTransferPeriod = scheduledTransferPeriod;

            

            // Performance counters
            
            diagnosticMonitorConfiguration.PerformanceCounters.DataSources.Add(new PerformanceCounterConfiguration()
            {
                CounterSpecifier = @"\Processor(_Total)\% Processor Time",
                SampleRate = sampleRate
                
            });

            diagnosticMonitorConfiguration.PerformanceCounters.DataSources.Add(new PerformanceCounterConfiguration()
            {
                CounterSpecifier = @"\ASP.NET\Application Restarts",
                SampleRate = sampleRate
            });

            diagnosticMonitorConfiguration.PerformanceCounters.DataSources.Add(new PerformanceCounterConfiguration()
            {
                CounterSpecifier = @"\ASP.NET\Requests Queued",
                SampleRate = sampleRate
            });

            diagnosticMonitorConfiguration.PerformanceCounters.DataSources.Add(new PerformanceCounterConfiguration()
            {
                CounterSpecifier = @"\ASP.NET\Requests Executing",
                SampleRate = sampleRate
            });

            diagnosticMonitorConfiguration.PerformanceCounters.DataSources.Add(new PerformanceCounterConfiguration()
            {
                CounterSpecifier = @"\ASP.NET\Request Wait Time",
                SampleRate = sampleRate
            });

            diagnosticMonitorConfiguration.PerformanceCounters.DataSources.Add(new PerformanceCounterConfiguration()
            {
                CounterSpecifier = @"\ASP.NET\Request Execution Time",
                SampleRate = sampleRate
            });

            diagnosticMonitorConfiguration.PerformanceCounters.DataSources.Add(new PerformanceCounterConfiguration()
            {
                CounterSpecifier = @"\ASP.NET\Requests Timed Out",
                SampleRate = sampleRate
            });

            diagnosticMonitorConfiguration.PerformanceCounters.DataSources.Add(new PerformanceCounterConfiguration()
            {
                CounterSpecifier = @"\ASP.NET Applications(__Total__)\Requests/Sec",
                SampleRate = sampleRate
            });

            diagnosticMonitorConfiguration.PerformanceCounters.DataSources.Add(new PerformanceCounterConfiguration()
            {
                CounterSpecifier = @"\ASP.NET\Anonymous Requests/Sec",
                SampleRate = sampleRate
            });
              diagnosticMonitorConfiguration.PerformanceCounters.DataSources.Add(new PerformanceCounterConfiguration()
            {
                CounterSpecifier = @"\ASP.NET\Errors Total/Sec",
                SampleRate = sampleRate
            });

            //  diagnosticMonitorConfiguration.PerformanceCounters.BufferQuotaInMB = 500;
            diagnosticMonitorConfiguration.PerformanceCounters.ScheduledTransferPeriod = scheduledTransferPeriod;

            DiagnosticMonitor.Start("Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString", diagnosticMonitorConfiguration);
        }
    }
}
