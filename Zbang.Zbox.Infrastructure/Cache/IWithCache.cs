using System;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Infrastructure.Cache
{
    public interface IWithCache
    {
        Task RemoveAsync<TC>(TC command)
            where TC : ICommandCache;
    }
}
