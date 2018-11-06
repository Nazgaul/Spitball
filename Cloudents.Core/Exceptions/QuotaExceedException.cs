using System;
using System.Runtime.Serialization;

namespace Cloudents.Core.Exceptions
{
    public class QuotaExceedException : Exception
    {
        public QuotaExceedException()
        {
        }

        public QuotaExceedException(string message) : base(message)
        {
        }

        public QuotaExceedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected QuotaExceedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}