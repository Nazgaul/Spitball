using System;
using System.Collections.Generic;

namespace Zbang.Zbox.Infrastructure.Trace
{
    public class Logger : ILogger
    {
       

        public void Exception(Exception ex, IDictionary<string, string> properties = null)
        {
            if (ex == null) throw new ArgumentNullException(nameof(ex));
            System.Diagnostics.Trace.TraceError(ex.ToString());
        }

        public void Info(string info)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(info);
#endif
            System.Diagnostics.Trace.TraceInformation(info);
        }

        public void Warning(string warning)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(warning);
#endif
            System.Diagnostics.Trace.TraceWarning(warning);
        }

        public void Error(string error)
        {
            System.Diagnostics.Trace.TraceError(error);
        }

        public void TrackMetric(string name, double value)
        {
            Info($"{name} metric value {value}");
        }

        public void Exception(string info, Exception ex)
        {
            if (ex == null) throw new ArgumentNullException(nameof(ex));
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"{info} \n {ex}");
#endif
            System.Diagnostics.Trace.TraceError($"{info} \n {ex} ");
        }
    }
}