using System;
using t = System.Diagnostics;

namespace Zbang.Zbox.Infrastructure.Trace
{
    [Obsolete("use ILogger")]
    public static class TraceLog
    {
        public static void WriteError(Exception ex)
        {
            if (ex == null) throw new ArgumentNullException(nameof(ex));
            t.Trace.TraceError(ex.ToString());
        }

        

        public static void WriteInfo(string info)
        {
#if DEBUG
            t.Debug.WriteLine(info);
#endif
            t.Trace.TraceInformation(info);
        }

        //public static void WriteInfo(string prefix, string info)
        //{
        //    WriteInfo(prefix + " " + info);
        //}

        public static void WriteError(string info, Exception ex)
        {
            #if DEBUG
            t.Debug.WriteLine($"{info} \n {ex}");
#endif
            t.Trace.TraceError($"{info} \n {ex} ");
        }

        public static void WriteError(string info, Exception ex, string additionalInfo)
        {
            t.Trace.TraceError($" {info} \n {ex} \n {additionalInfo}");
        }

        public static void WriteError(string error)
        {
            t.Trace.TraceError(error);

        }

        public static void WriteWarning(string warning)
        {
#if DEBUG
            t.Debug.WriteLine(warning);
#endif
            t.Trace.TraceWarning(warning);
        }

    }
}
