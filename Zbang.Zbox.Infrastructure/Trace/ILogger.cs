using System;
using System.Collections.Generic;

namespace Zbang.Zbox.Infrastructure.Trace
{
    public interface ILogger
    {
        void Exception(Exception ex , IDictionary<string, string> properties = null);

        void Info(string info);

        void Warning(string warning);

        void Error(string error);

        void TrackMetric(string name, double value);



    }
}