using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Exceptions
{
    [Serializable]
    public class UserRegisterFacebookException : Exception
    {
    }

    [Serializable]
    public class UserRegisterGoogleException : Exception
    {
    }

    [Serializable]
    public class UserRegisterEmailException : Exception
    {
    }
}
