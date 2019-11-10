using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Cloudents.Core
{
    public class Logger : ILogger
    {
        public void Exception(Exception ex, IDictionary<string, string> properties = null)
        {
            if (ex == null) throw new ArgumentNullException(nameof(ex));
            System.Diagnostics.Trace.TraceError(ex.ToString());
        }

        public void Info(string message, bool email = false)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(message);
#endif
            System.Diagnostics.Trace.TraceInformation(message);
        }

        public void Warning(string message)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(message);
#endif
            System.Diagnostics.Trace.TraceWarning(message);
        }

        public void Error(string message)
        {
            System.Diagnostics.Trace.TraceError(message);
        }

        public void TrackMetric(string name, double value)
        {
            Info($"{name} metric value {value}");
        }

        //        public void Exception(string info, Exception ex)
        //        {
        //            if (ex == null) throw new ArgumentNullException(nameof(ex));
        //#if DEBUG
        //            System.Diagnostics.Debug.WriteLine($"{info} \n {ex}");
        //#endif
        //            System.Diagnostics.Trace.TraceError($"{info} \n {ex} ");
        //        }
    }
}