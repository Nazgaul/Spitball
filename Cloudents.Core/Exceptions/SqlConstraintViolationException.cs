using System;
using System.Runtime.Serialization;

namespace Cloudents.Core.Exceptions
{
    public class SqlConstraintViolationException : Exception
    {
        public SqlConstraintViolationException()
        {
        }

        public SqlConstraintViolationException(string message) : base(message)
        {
        }

        public SqlConstraintViolationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SqlConstraintViolationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}