using System;

namespace Cloudents.Core.Interfaces
{
    public interface ICacheProvider
    {
        object Get(string key, string region);
        void Set(string key, string region, object value, int expire);
    }


}