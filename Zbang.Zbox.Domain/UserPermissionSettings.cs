using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.Domain
{
    [Flags]
    public enum UserPermissionSettings
    {
        Read = 1,
        ReadWrite = 2,
        Manager = 4,
        Owner = 8
        
    }

   
}
