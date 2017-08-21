using System;

namespace Zbang.Zbox.Infrastructure.IdGenerator
{
    public interface IIdGenerator
    {
        long GetId(string scopeName);
    }

    public interface IGuidIdGenerator
    {
        Guid GetId();
    }

    
}
