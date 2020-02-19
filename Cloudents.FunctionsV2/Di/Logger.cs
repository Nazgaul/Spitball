using Cloudents.Core.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using System;
using System.Collections.Generic;

namespace Cloudents.FunctionsV2.Di
{
    public class Logger : ILogger
    {
        private readonly TelemetryClient _telemetry = new TelemetryClient();
        public void Exception(Exception ex, IDictionary<string, string> properties = null)
        {
            _telemetry.TrackException(ex, properties);
        }

        public void Info(string message, bool email = false)
        {
            _telemetry.TrackTrace(message);
        }

        public void Warning(string message)
        {
            _telemetry.TrackTrace(message, SeverityLevel.Warning);
        }

        public void Error(string message)
        {
            _telemetry.TrackTrace(message, SeverityLevel.Error);
        }

        public void TrackMetric(string name, double value)
        {
            _telemetry.TrackMetric(name, value);
        }
    }
}