using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Cloudents.FunctionsV2.Di
{
    public class Logger : Cloudents.Core.Interfaces.ILogger
    {
        private readonly ILogger _logger;

        public Logger(ILogger logger)
        {
            _logger = logger;
        }

        public void Exception(Exception ex, IDictionary<string, string> properties = null)
        {
            _logger.LogCritical(ex,null,null);
        }

        public void Info(string message, bool email = false)
        {
            _logger.LogInformation(message);
        }

        public void Warning(string message)
        {
            _logger.LogWarning(message);

        }

        public void Error(string message)
        {
            _logger.LogError(message);
        }

        public void TrackMetric(string name, double value)
        {
            // throw new NotImplementedException();
        }
    }
}