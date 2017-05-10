using System;
using System.Runtime.Serialization;

namespace Zbang.Zbox.Infrastructure.Exceptions
{
    [Serializable]
    public class PushFailedException : Exception
    {
        public PushFailedException()
        {
        }

        public PushFailedException(string message) : base(message)
        {
        }

        public PushFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PushFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
