using System;
using Cloudents.Core.Interfaces;

namespace Cloudents.Infrastructure.Data.Test.IntegrationTests
{
    public class DummyCacheProvider : ICacheProvider
    {
        public object? Get(string key, string region)
        {
            return null;
        }

        public T Get<T>(string key, string region)
        {
            return default;
        }

        public void Set(string key, string region, object value, int expire, bool slideExpiration)
        {

        }

        public void Set<T>(string key, string region, T value, TimeSpan expire, bool slideExpiration)
        {
           
        }

        public void Set(string key, string region, object value, TimeSpan expire, bool slideExpiration)
        {

        }

        public bool Exists(string key, string region)
        {
            return false;
        }

        public void DeleteRegion(string region)
        {

        }

        public void DeleteKey(string region, string key)
        {

        }
    }
}