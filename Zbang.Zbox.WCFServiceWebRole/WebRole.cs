using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace Zbang.Zbox.WCFServiceWebRole
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            // To enable the AzureLocalStorageTraceListner, uncomment relevent section in the web.config  
            //DiagnosticMonitorConfiguration diagnosticConfig = DiagnosticMonitor.GetDefaultInitialConfiguration();
            //diagnosticConfig.Directories.ScheduledTransferPeriod = TimeSpan.FromMinutes(1);
            //diagnosticConfig.Directories.DataSources.Add(AzureLocalStorageTraceListener.GetLogDirectory());

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
            //this.ConfigureDiagnostics();
            return base.OnStart();
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

            ////?
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

            // Add a performance counter for requests per second.
            diagnosticMonitorConfiguration.PerformanceCounters.DataSources.Add(new PerformanceCounterConfiguration()
            {
                CounterSpecifier = @"\ASP.NET\Requests/Sec",
                SampleRate = TimeSpan.FromMinutes(5)
            });


            diagnosticMonitorConfiguration.PerformanceCounters.ScheduledTransferPeriod = scheduledTransferPeriod;

            DiagnosticMonitor.Start("Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString", diagnosticMonitorConfiguration);    
        }
    }
}
