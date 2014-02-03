using System;

namespace Zbang.Zbox.Infrastructure.Exceptions
{
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
