using System;
using System.Collections.Generic;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class TelemetryLogger : ILogger
    {
        private readonly TelemetryClient m_Telemetry = new TelemetryClient();

        public void Exception(Exception ex, IDictionary<string, string> properties = null)
        {
            m_Telemetry.TrackException(ex, properties);
        }

        public void Info(string info)
        {
            m_Telemetry.TrackTrace(info, SeverityLevel.Information);
        }

        public void Warning(string warning)
        {
            m_Telemetry.TrackTrace(warning, SeverityLevel.Warning);
        }

        public void Error(string error)
        {
            m_Telemetry.TrackTrace(error, SeverityLevel.Error);
        }

        public void TrackMetric(string name, double value)
        {
            var sample = new MetricTelemetry
            {
                Name = name,
                Sum = value
            };
            m_Telemetry.TrackMetric(sample);
        }
    }
}
