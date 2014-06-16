using System;
using t = System.Diagnostics;

namespace Zbang.Zbox.Infrastructure.Trace
{
    public static class TraceLog
    {
        public static void WriteError(Exception ex)
        {
            t.Trace.TraceError(ex.ToString());
        }
        public static void WriteInfo(Exception ex)
        {
            t.Trace.TraceInformation(ex.ToString());
        }

        public static void WriteInfo(string info)
        {
            t.Trace.TraceInformation(info);
        }

        public static void WriteError(string info, Exception ex)
        {
            t.Trace.TraceError(string.Format(" {0} \n {1}", info, ex));
        }

        public static void WriteError(string info, Exception ex, string additionalInfo)
        {
            t.Trace.TraceError(string.Format(" {0} \n {1} \n {2}", info, ex, additionalInfo));
        }

        public static void WriteError(string error)
        {
            t.Trace.TraceError(error);

        }

    }
}
