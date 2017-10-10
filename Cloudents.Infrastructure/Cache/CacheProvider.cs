using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CacheManager.Core;
using Castle.DynamicProxy;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Core.Request;

namespace Cloudents.Infrastructure.Cache
{
    public class CacheProvider<T> : ICacheProvider<T>
    {
        private readonly ICacheManager<T> m_Cache;

        public CacheProvider(ICacheManager<T> cache)
        {
            m_Cache = cache;
        }

        public T Get(IQuery key, CacheRegion region)
        {
            return m_Cache.Get(key.CacheKey, region.Name);
        }

        public void Set(IQuery key, CacheRegion region, T value, TimeSpan expire)
        {
            var cacheItem = new CacheItem<T>(key.CacheKey, region.Name, value, ExpirationMode.Sliding, expire);
            m_Cache.Put(cacheItem);
        }

    }

    public class CacheResultInterceptor : IInterceptor
    {
        private readonly ICacheProvider<object> m_CacheProvider;

        public CacheResultInterceptor(ICacheProvider<object> cacheProvider)
        {
            m_CacheProvider = cacheProvider;
        }

        public CacheResultAttribute GetCacheResultAttribute(IInvocation invocation)
        {
            return Attribute.GetCustomAttribute(
                    invocation.MethodInvocationTarget,
                    typeof(CacheResultAttribute)
                )
                as CacheResultAttribute;
        }

        public IQuery GetInvocationSignature(IInvocation invocation)
        {
            return invocation.Arguments.OfType<IQuery>().First();
            //return String.Format("{0}-{1}-{2}",
            //    invocation.TargetType.FullName,
            //    invocation.Method.Name,
            //    String.Join("-", invocation.Arguments.Select(a => (a ?? "").ToString()).ToArray())
            //);
        }

        private static async Task InterceptAsync(IQuery key, Task task)
        {
            await task.ConfigureAwait(false);
            // do the logging here, as continuation work for Task...
        }

        private async Task<T> InterceptAsync<T>(IQuery key, Task<T> task)
        {
            T result = await task.ConfigureAwait(false);
            m_CacheProvider.Set(key, CacheRegion.Ai, result, TimeSpan.FromHours(1));// cacheAttr.Duration);
            // do the logging here, as continuation work for Task<T>...
            return result;
        }

        public void Intercept(IInvocation invocation)
        {
            var cacheAttr = GetCacheResultAttribute(invocation);
            var key = GetInvocationSignature(invocation);

            var method = invocation.MethodInvocationTarget;
            var isAsync = method.GetCustomAttribute(typeof(AsyncStateMachineAttribute)) != null;

            var data = m_CacheProvider.Get(key, CacheRegion.Ai);
            if (data != null && (isAsync && typeof(Task).IsAssignableFrom(method.ReturnType)))
            {
                invocation.ReturnValue = Task.FromResult((dynamic)data); //InterceptAsync();
                return;
            }
            invocation.Proceed();

            if (isAsync && typeof(Task).IsAssignableFrom(method.ReturnType))
            {
                invocation.ReturnValue = InterceptAsync(key, (dynamic)invocation.ReturnValue);

            }


            //var cacheAttr = GetCacheResultAttribute(invocation);

            //if (cacheAttr == null)
            //{
            //    invocation.Proceed();
            //    return;
            //}

            //var key = GetInvocationSignature(invocation);

            //var data = m_CacheProvider.Get(key, CacheRegion.Ai);// cacheAttr.Region);
            //if (data != null)
            //{
            //    if (invocation.Method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
            //    {
            //        var resultType = invocation.Method.ReturnType.GetGenericArguments()[0];

            //        var convertedResult = Convert.ChangeType(data, resultType) ;

            //        invocation.ReturnValue = Task.FromResult(convertedResult);
            //        return;
            //    }
            //    invocation.ReturnValue = data;
            //    return;
            //}

            //invocation.Proceed();
            //var result = invocation.ReturnValue;

            //if (result != null)
            //{
            //    m_CacheProvider.Set(key, CacheRegion.Ai, result, TimeSpan.FromHours(1));// cacheAttr.Duration);
            //}
        }
    }
}
