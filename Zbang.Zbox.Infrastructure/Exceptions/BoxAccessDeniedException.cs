using System;

namespace Zbang.Zbox.Infrastructure.Exceptions
{
    [Serializable]
    public class BoxAccessDeniedException : Exception
    {
    }

    [Serializable]
    public class DuplicateDepartmentNameException : Exception
    {
    }

    [Serializable]
    public class BoxesInDepartmentNodeException : Exception
    {
    }
}
