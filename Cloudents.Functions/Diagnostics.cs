using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Queue;
using System;

namespace Cloudents.Functions
{
    public static class Diagnostics
    {
        private static string key = TelemetryConfiguration.Active.InstrumentationKey
            = System.Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY",
                EnvironmentVariableTarget.Process);
        private static TelemetryClient telemetry = new TelemetryClient() { InstrumentationKey = key };

        [FunctionName("Diagnostics")]
        public static void Run(
            [TimerTrigger("0 10,20,30,40,50 * * * *")]TimerInfo myTimer,
            [Queue("generate-blob-preview")] CloudQueue queue,
            TraceWriter log)
        {
            queue.FetchAttributes();
            var count = queue.ApproximateMessageCount;

            telemetry.TrackMetric("queueGenerateBlobPreview", count.GetValueOrDefault());
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
