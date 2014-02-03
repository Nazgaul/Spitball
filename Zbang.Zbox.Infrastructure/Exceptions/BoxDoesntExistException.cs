using System;

namespace Zbang.Zbox.Infrastructure.Exceptions
{
    [Serializable]
    public class BoxDoesntExistException : Exception
    {
        public BoxDoesntExistException() { }
        public BoxDoesntExistException(string message) : base(message) { }
        public BoxDoesntExistException(string message, Exception inner) : base(message, inner) { }
        protected BoxDoesntExistException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
