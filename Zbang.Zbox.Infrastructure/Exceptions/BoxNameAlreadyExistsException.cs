using System;

namespace Zbang.Zbox.Infrastructure.Exceptions
{
    [Serializable]
    public class BoxNameAlreadyExistsException : ArgumentException
    {
        public BoxNameAlreadyExistsException()
            : base("Box name already exists")
        {

        }
    }
}
