
namespace Zbang.Zbox.Infrastructure.Cache
{
    public interface IHttpContextCacheWrapper
    {
        object GetObject(string key);
        void AddObject(string key, object value);
    }
}
