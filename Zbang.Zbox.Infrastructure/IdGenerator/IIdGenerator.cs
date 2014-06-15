using System;

namespace Zbang.Zbox.Infrastructure.IdGenerator
{
    public interface IIdGenerator
    {
        long GetId(string scopeName);
        Guid GetId();
        
    }

   

    
}
