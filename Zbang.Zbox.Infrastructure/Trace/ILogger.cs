using System;
using System.Collections.Generic;

namespace Zbang.Zbox.Infrastructure.Trace
{
    public interface ILogger
    {
        void Exception(Exception ex , IDictionary<string, string> properties = null);

        void Info(string message);

        void Warning(string message);

        void Error(string message);

        void TrackMetric(string name, double value);
    }
}