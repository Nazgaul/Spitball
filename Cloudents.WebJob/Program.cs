using System;
using System.Diagnostics;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Cloudents.WebJob
{
    // To learn more about Microsoft Azure WebJobs SDK, please see https://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            var config = new JobHostConfiguration
            {
                JobActivator = new MyActivator(),
                Queues=
                {
                    BatchSize = 32,
                    MaxDequeueCount = 5
                }
            };
            string instrumentationKey = Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY");
            if (!string.IsNullOrEmpty(instrumentationKey))
            {
                // Wire up with default filters; Filtering will be explained later.
                config.LoggerFactory = new LoggerFactory()
                    .AddApplicationInsights(instrumentationKey, null)
                    .AddConsole();

                config.Tracing.ConsoleLevel = TraceLevel.Off;
            }

            if (config.IsDevelopment)
            {
                config.UseDevelopmentSettings();
            }

            config.UseTimers();
            var host = new JobHost(config);
            // The following code ensures that the WebJob will be running continuously
            host.RunAndBlock();
        }
    }
}
