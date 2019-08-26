using System;
using System.Runtime.Serialization;

namespace Cloudents.Infrastructure.Framework
{
    public class PreviewFailedException : Exception
    {
        public PreviewFailedException()
        {
        }

        public PreviewFailedException(string message) : base(message)
        {
        }

        public PreviewFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PreviewFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}