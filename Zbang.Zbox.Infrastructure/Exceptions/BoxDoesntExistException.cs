using System;

namespace Zbang.Zbox.Infrastructure.Exceptions
{
    [Serializable]
    public class BoxDoesntExistException : Exception
    {
        public BoxDoesntExistException() { }
        protected BoxDoesntExistException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
