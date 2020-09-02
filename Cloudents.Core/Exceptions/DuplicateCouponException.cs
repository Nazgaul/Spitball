using System;
using System.Runtime.Serialization;

namespace Cloudents.Core.Exceptions
{
    public class DuplicateCouponException : Exception
    {
        public DuplicateCouponException()
        {
        }

        public DuplicateCouponException(string message) : base(message)
        {
        }

        public DuplicateCouponException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DuplicateCouponException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}