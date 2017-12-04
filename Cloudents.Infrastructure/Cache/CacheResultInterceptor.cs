using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;

namespace Cloudents.Infrastructure.Cache
{
    public class CacheResultInterceptor : IInterceptor
    {
        private readonly ICacheProvider _cacheProvider;

        public CacheResultInterceptor(ICacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
        }


        public static CacheAttribute GetCacheResultAttribute(IInvocation invocation)
        {
            return Attribute.GetCustomAttribute(
                    invocation.MethodInvocationTarget,
                    typeof(CacheAttribute)
                )
                as CacheAttribute;
        }

        public static string GetInvocationSignature(IInvocation invocation)
        {
            return
                $"{invocation.TargetType.FullName}-{invocation.Method.Name}-{string.Join("-", invocation.Arguments.Select(a => (a ?? "").ToString()).ToArray())}";
        }

        private static Task InterceptAsync(string key, CacheAttribute att, Task task)
        {
            return task;
            // do the logging here, as continuation work for Task...
        }

        private async Task<T> InterceptAsync<T>(string key, CacheAttribute att, Task<T> task)
        {
            var result = await task.ConfigureAwait(false);
            if (result != null)
            {
                _cacheProvider.Set(key, att.Region, result, att.Duration); // cacheAttr.Duration);
            }
            // do the logging here, as continuation work for Task<T>...
            return result;
        }

        public static Task<T> ConvertAsync<T>(T data)
        {
            return Task.FromResult(data);
        }

        public void Intercept(IInvocation invocation)
        {
            var cacheAttr = GetCacheResultAttribute(invocation);
            if (cacheAttr == null)
            {
                invocation.Proceed();
                return;
            }
            var key = GetInvocationSignature(invocation);

            var method = invocation.MethodInvocationTarget;
            var isAsync = method.GetCustomAttribute(typeof(AsyncStateMachineAttribute)) != null;
            var data = _cacheProvider.Get(key, cacheAttr.Region);

            if (data != null)
            {
                if (isAsync && typeof(Task).IsAssignableFrom(method.ReturnType))
                {
                    var taskReturnType = method.ReturnType; //e.g. Task<int>

                    var type = taskReturnType.GetGenericArguments()[0]; //get the result type, e.g. int

                    var convertMethod =
                        GetType().GetMethod(nameof(ConvertAsync))
                            .MakeGenericMethod(type); //Get the closed version of the Convert method, e.g. Convert<int>

                    var result =
                        convertMethod.Invoke(null,
                            new[] { data }); //Call the convert method and return the generic Task, e.g. Task<int>

                    invocation.ReturnValue = result;
                    return;
                }
                invocation.ReturnValue = data;
            }
            invocation.Proceed();

            if (isAsync && typeof(Task).IsAssignableFrom(method.ReturnType))
            {
                invocation.ReturnValue = InterceptAsync(key, cacheAttr, (dynamic)invocation.ReturnValue);
            }
        }
    }
}