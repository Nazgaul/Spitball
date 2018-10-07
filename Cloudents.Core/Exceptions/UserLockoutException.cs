using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cloudents.Core.Exceptions
{
    public class UserLockoutException : Exception
    {
        public UserLockoutException()
        {
        }

        public UserLockoutException(string message) : base(message)
        {
        }

        public UserLockoutException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UserLockoutException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
