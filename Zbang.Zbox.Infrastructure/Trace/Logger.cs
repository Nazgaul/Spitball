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

        public void Info(string message)
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
    }
}