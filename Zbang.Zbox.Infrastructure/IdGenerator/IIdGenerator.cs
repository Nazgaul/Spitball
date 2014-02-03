using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.Infrastructure.IdGenerator
{
    public interface IIdGenerator
    {
        long GetId(string scopeName);
        Guid GetId();
        
    }

   

    
}
