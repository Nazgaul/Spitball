using System;

namespace Zbang.Zbox.Infrastructure.Exceptions
{
    [Serializable]
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(string error): base(error)
        {

        }

        public UserNotFoundException()
        {

        }
    }
}
