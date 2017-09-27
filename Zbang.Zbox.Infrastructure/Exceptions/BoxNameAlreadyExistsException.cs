using System;

namespace Zbang.Zbox.Infrastructure.Exceptions
{
    [Serializable]
    public class BoxNameAlreadyExistsException : ArgumentException
    {
        public long BoxId { get; }

        public BoxNameAlreadyExistsException(long boxId)
            : base("Box name already exists")
        {
            BoxId = boxId;
        }
    }
}
