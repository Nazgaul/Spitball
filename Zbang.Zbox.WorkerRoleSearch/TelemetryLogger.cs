using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
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
            m_Telemetry.TrackTrace(info, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Information);
        }

        public void Warning(string warning)
        {
            m_Telemetry.TrackTrace(warning, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Warning);
        }

        public void Error(string error)
        {
            m_Telemetry.TrackTrace(error, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Error);

        }
    }
}
