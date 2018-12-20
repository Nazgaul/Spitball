using System;
using System.Runtime.Serialization;

namespace Cloudents.Core.Exceptions
{
    public class DuplicateRowException : Exception
    {
        public DuplicateRowException()
        {
        }

        public DuplicateRowException(string message) : base(message)
        {
        }

        public DuplicateRowException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DuplicateRowException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}