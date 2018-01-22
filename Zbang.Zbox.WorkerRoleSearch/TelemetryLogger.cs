using System;
using System.Collections.Generic;
//using Microsoft.ApplicationInsights;
//using Microsoft.ApplicationInsights.DataContracts;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class TelemetryLogger : ILogger
    {
        //private readonly TelemetryClient _telemetry = new TelemetryClient();

        public void Exception(Exception ex, IDictionary<string, string> properties = null)
        {
            //_telemetry.TrackException(ex, properties);
        }

        public void Info(string message)
        {
           // _telemetry.TrackTrace(message, SeverityLevel.Information);
        }

        public void Warning(string message)
        {
            //_telemetry.TrackTrace(message, SeverityLevel.Warning);
        }

        public void Error(string message)
        {
            //_telemetry.TrackTrace(message, SeverityLevel.Error);
        }

        public void TrackMetric(string name, double value)
        {
            //var sample = new MetricTelemetry
            //{
            //    Name = name,
            //    Sum = value
            //};
            //_telemetry.TrackMetric(sample);
        }
    }
}
