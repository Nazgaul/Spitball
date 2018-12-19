using System;
using System.Collections.Generic;

namespace Cloudents.Application.Interfaces
{
    public interface ILogger
    {
        void Exception(Exception ex, IDictionary<string, string> properties = null);

        void Info(string message, bool email = false);

        void Warning(string message);

        void Error(string message);

        void TrackMetric(string name, double value);
    }
}