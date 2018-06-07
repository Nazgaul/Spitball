using System;

namespace Cloudents.Web.Identity
{
    public class UserNameExistsException : Exception
    {
        public UserNameExistsException() : base()
        {
        }

        public UserNameExistsException(string message) : base(message)
        {
        }

        public UserNameExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UserNameExistsException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
    }
}
