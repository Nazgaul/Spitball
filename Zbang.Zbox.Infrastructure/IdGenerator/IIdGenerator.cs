using System;

namespace Zbang.Zbox.Infrastructure.IdGenerator
{
    public interface IGuidIdGenerator
    {
        Guid GetId();
    }
}
