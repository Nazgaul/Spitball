using System;
using System.Collections.Generic;

namespace Cloudents.Core.Interfaces
{
    public interface ILogger
    {
        void Exception(Exception ex, IDictionary<string, string>? properties = null);

        void Info(string message, IDictionary<string, string>? properties = null);

        void Warning(string message);

        //void Error(string message);

        void Error(string message, IDictionary<string, string>? properties = null);

       // void TrackMetric(string name, double value);
    }
}