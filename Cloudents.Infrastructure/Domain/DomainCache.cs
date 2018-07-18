using System.Threading.Tasks;
using Cloudents.Core;
using JetBrains.Annotations;
using Nager.PublicSuffix;

namespace Cloudents.Infrastructure.Domain
{
    [UsedImplicitly]
    public class DomainCache : ICacheProvider
    {
        private readonly Core.Interfaces.ICacheProvider _cacheProvider;

        private const string Key = "suffix";
        private const string Region = "domain";

        public DomainCache(Core.Interfaces.ICacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
        }

        public Task<string> GetValueAsync()
        {
            if (_cacheProvider.Get(Key, Region) is string p)
            {
                return Task.FromResult(p);
            }

            return Task.FromResult<string>(null);
        }

        public Task SetValueAsync(string val)
        {
            _cacheProvider.Set(Key, Region, val, TimeConst.Month, true);
            return Task.CompletedTask;
        }

        public bool IsCacheValid()
        {
            return _cacheProvider.Exists(Key, Region);
        }
    }
}